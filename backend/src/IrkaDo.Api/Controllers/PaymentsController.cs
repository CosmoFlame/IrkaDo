using IrkaDo.Application.Common.Interfaces;
using IrkaDo.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IrkaDo.Api.Controllers;

public record CheckoutResponseDto(string CheckoutUrl);

public record PurchaseStatusDto(string Status, string GuideTitle, string? DownloadUrl);

[ApiController]
[Route("api/v1")]
public class PaymentsController : ControllerBase
{
    /// <summary>Expiry for the download link surfaced on the confirmation screen (regenerated per poll).</summary>
    private static readonly TimeSpan ConfirmationLinkExpiry = TimeSpan.FromHours(1);

    private readonly IAppDbContext _db;
    private readonly IPaymentProvider _paymentProvider;
    private readonly IGuideDeliveryQueue _deliveryQueue;
    private readonly IFileStorageService _storage;
    private readonly IConfiguration _configuration;
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(
        IAppDbContext db,
        IPaymentProvider paymentProvider,
        IGuideDeliveryQueue deliveryQueue,
        IFileStorageService storage,
        IConfiguration configuration,
        ILogger<PaymentsController> logger)
    {
        _db = db;
        _paymentProvider = paymentProvider;
        _deliveryQueue = deliveryQueue;
        _storage = storage;
        _configuration = configuration;
        _logger = logger;
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

        var siteUrl = (_configuration["SiteUrl"] ?? "http://localhost:3000").TrimEnd('/');

        // {CHECKOUT_SESSION_ID} is a Stripe template token replaced with the real session id on
        // redirect, letting the confirmation screen look the purchase up via the status endpoint.
        var session = await _paymentProvider.CreateCheckoutSessionAsync(new CheckoutSessionRequest(
            guide.Id, guide.Title, guide.PriceAmount.Value, guide.PriceCurrency,
            $"{siteUrl}/guides/{guide.Slug}?purchase=success&session_id={{CHECKOUT_SESSION_ID}}",
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

        PaymentWebhookEvent webhookEvent;
        try
        {
            webhookEvent = await _paymentProvider.ParseWebhookAsync(payload, signature, cancellationToken);
        }
        catch (Exception ex)
        {
            // Invalid signature or malformed payload — reject so the provider can surface the error.
            _logger.LogWarning(ex, "Rejected payment webhook: signature/payload validation failed.");
            return BadRequest();
        }

        var purchase = await _db.Purchases
            .FirstOrDefaultAsync(p => p.ProviderSessionId == webhookEvent.SessionId, cancellationToken);

        if (purchase is null)
            return Ok();

        if (webhookEvent.Type == PaymentWebhookEventType.CheckoutCompleted)
        {
            // Idempotency: a completed purchase has already been persisted and queued for delivery;
            // Stripe re-sends events, so ignore duplicates rather than enqueueing/charging twice.
            if (purchase.Status == PurchaseStatus.Completed)
                return Ok();

            purchase.Status = PurchaseStatus.Completed;
            purchase.ProviderPaymentId = webhookEvent.PaymentId;
            purchase.CustomerEmail = webhookEvent.CustomerEmail ?? purchase.CustomerEmail;
            await _db.SaveChangesAsync(cancellationToken);

            // Persist first, then hand delivery off to the background pipeline so email problems
            // never turn into a webhook 500 (which would make Stripe retry the whole event).
            _deliveryQueue.Enqueue(purchase.Id);
        }
        else if (webhookEvent.Type == PaymentWebhookEventType.PaymentFailed)
        {
            if (purchase.Status == PurchaseStatus.Pending)
            {
                purchase.Status = PurchaseStatus.Failed;
                await _db.SaveChangesAsync(cancellationToken);
            }
        }

        return Ok();
    }

    [HttpGet("purchases/{sessionId}")]
    public async Task<ActionResult<PurchaseStatusDto>> GetStatus(
        string sessionId, CancellationToken cancellationToken = default)
    {
        var purchase = await _db.Purchases
            .Include(p => p.TravelGuide)
            .ThenInclude(g => g!.Files)
            .FirstOrDefaultAsync(p => p.ProviderSessionId == sessionId, cancellationToken);

        if (purchase is null)
            return NotFound();

        var guideTitle = purchase.TravelGuide?.Title ?? string.Empty;

        string? downloadUrl = null;
        if (purchase.Status == PurchaseStatus.Completed)
        {
            var file = purchase.TravelGuide?.Files.FirstOrDefault();
            if (file is not null)
            {
                downloadUrl = await _storage.GetSignedDownloadUrlAsync(
                    file.StorageKey, file.FileName, ConfirmationLinkExpiry,
                    $"{Request.Scheme}://{Request.Host}", cancellationToken);
            }
        }

        return Ok(new PurchaseStatusDto(
            purchase.Status.ToString().ToLowerInvariant(), guideTitle, downloadUrl));
    }
}
