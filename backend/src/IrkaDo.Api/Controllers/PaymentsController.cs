using IrkaDo.Application.Common.Interfaces;
using IrkaDo.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IrkaDo.Api.Controllers;

public record CheckoutResponseDto(string CheckoutUrl);

[ApiController]
[Route("api/v1")]
public class PaymentsController : ControllerBase
{
    private readonly IAppDbContext _db;
    private readonly IPaymentProvider _paymentProvider;
    private readonly IEmailSender _emailSender;
    private readonly IConfiguration _configuration;

    public PaymentsController(
        IAppDbContext db, IPaymentProvider paymentProvider, IEmailSender emailSender, IConfiguration configuration)
    {
        _db = db;
        _paymentProvider = paymentProvider;
        _emailSender = emailSender;
        _configuration = configuration;
    }

    [HttpPost("guides/{slug}/checkout")]
    public async Task<ActionResult<CheckoutResponseDto>> Checkout(
        string slug, CancellationToken cancellationToken = default)
    {
        var guide = await _db.TravelGuides
            .FirstOrDefaultAsync(g => g.IsPublished && g.Slug == slug, cancellationToken);

        if (guide is null)
            return NotFound();

        if (!guide.IsPremium || guide.PriceAmount is null)
            return BadRequest("This guide is not a paid guide.");

        var siteUrl = _configuration["SiteUrl"] ?? "http://localhost:3000";
        var session = await _paymentProvider.CreateCheckoutSessionAsync(new CheckoutSessionRequest(
            guide.Id, guide.Title, guide.PriceAmount.Value, guide.PriceCurrency,
            $"{siteUrl}/guides/{guide.Slug}?purchase=success",
            $"{siteUrl}/guides/{guide.Slug}?purchase=cancelled"), cancellationToken);

        _db.Purchases.Add(new Purchase
        {
            TravelGuideId = guide.Id,
            CustomerEmail = string.Empty,
            AmountPaid = guide.PriceAmount.Value,
            Currency = guide.PriceCurrency,
            Status = PurchaseStatus.Pending,
            ProviderSessionId = session.SessionId
        });
        await _db.SaveChangesAsync(cancellationToken);

        return Ok(new CheckoutResponseDto(session.CheckoutUrl));
    }

    [HttpPost("payments/webhook")]
    public async Task<IActionResult> Webhook(CancellationToken cancellationToken = default)
    {
        using var reader = new StreamReader(Request.Body);
        var payload = await reader.ReadToEndAsync(cancellationToken);
        var signature = Request.Headers["Stripe-Signature"].ToString();

        var webhookEvent = await _paymentProvider.ParseWebhookAsync(payload, signature, cancellationToken);

        var purchase = await _db.Purchases
            .Include(p => p.TravelGuide)
            .ThenInclude(g => g!.Files)
            .FirstOrDefaultAsync(p => p.ProviderSessionId == webhookEvent.SessionId, cancellationToken);

        if (purchase is null)
            return Ok();

        if (webhookEvent.Type == Application.Common.Interfaces.PaymentWebhookEventType.CheckoutCompleted)
        {
            purchase.Status = PurchaseStatus.Completed;
            purchase.ProviderPaymentId = webhookEvent.PaymentId;
            purchase.CustomerEmail = webhookEvent.CustomerEmail ?? purchase.CustomerEmail;
            purchase.DeliveredAt = DateTimeOffset.UtcNow;

            if (!string.IsNullOrWhiteSpace(purchase.CustomerEmail))
            {
                await _emailSender.SendAsync(
                    purchase.CustomerEmail,
                    $"Your guide is ready: {purchase.TravelGuide?.Title}",
                    $"<p>Thank you for your purchase! Download your guide here: " +
                    $"{purchase.TravelGuide?.Files.FirstOrDefault()?.StorageKey}</p>",
                    cancellationToken);
            }
        }
        else if (webhookEvent.Type == Application.Common.Interfaces.PaymentWebhookEventType.PaymentFailed)
        {
            purchase.Status = PurchaseStatus.Failed;
        }

        await _db.SaveChangesAsync(cancellationToken);
        return Ok();
    }
}
