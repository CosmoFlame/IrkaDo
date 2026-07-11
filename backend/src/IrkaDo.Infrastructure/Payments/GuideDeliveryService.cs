using System.Net;
using IrkaDo.Application.Common.Interfaces;
using IrkaDo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IrkaDo.Infrastructure.Payments;

public class GuideDeliveryService : IGuideDeliveryService
{
    /// <summary>How long an emailed guide download link stays valid.</summary>
    public static readonly TimeSpan EmailedLinkExpiry = TimeSpan.FromDays(30);

    private readonly IAppDbContext _db;
    private readonly IEmailSender _emailSender;
    private readonly IFileStorageService _storage;
    private readonly ILogger<GuideDeliveryService> _logger;

    public GuideDeliveryService(
        IAppDbContext db,
        IEmailSender emailSender,
        IFileStorageService storage,
        ILogger<GuideDeliveryService> logger)
    {
        _db = db;
        _emailSender = emailSender;
        _storage = storage;
        _logger = logger;
    }

    public async Task DeliverAsync(Guid purchaseId, CancellationToken cancellationToken = default)
    {
        var purchase = await _db.Purchases
            .Include(p => p.TravelGuide)
            .ThenInclude(g => g!.Files)
            .FirstOrDefaultAsync(p => p.Id == purchaseId, cancellationToken);

        if (purchase is null)
        {
            _logger.LogWarning("Delivery skipped: purchase {PurchaseId} not found.", purchaseId);
            return;
        }

        // Idempotency guard: only deliver a completed, not-yet-delivered purchase. Protects against
        // duplicate webhook events and background-service retries re-sending the email.
        if (purchase.Status != PurchaseStatus.Completed || purchase.DeliveredAt is not null)
            return;

        if (string.IsNullOrWhiteSpace(purchase.CustomerEmail))
        {
            _logger.LogWarning(
                "Delivery skipped: purchase {PurchaseId} has no customer email.", purchaseId);
            return;
        }

        var file = purchase.TravelGuide?.Files.FirstOrDefault();
        if (file is null)
        {
            _logger.LogError(
                "Delivery failed: guide {GuideId} for purchase {PurchaseId} has no downloadable file.",
                purchase.TravelGuideId, purchaseId);
            return;
        }

        // Emailed links are built by a background worker (no HTTP request), so this keeps the
        // configured API base URL rather than a request-derived one.
        var downloadUrl = await _storage.GetSignedDownloadUrlAsync(
            file.StorageKey, file.FileName, EmailedLinkExpiry, cancellationToken: cancellationToken);

        var title = WebUtility.HtmlEncode(purchase.TravelGuide!.Title);
        var body =
            $"""
             <h2>Thank you for your purchase!</h2>
             <p>Your guide <strong>{title}</strong> is ready to download.</p>
             <p><a href="{downloadUrl}">Download your guide</a></p>
             <p style="color:#666;font-size:13px">This link is valid for 30 days. If it expires,
             just reply to this email and we'll send a fresh one.</p>
             <p>Happy travels,<br>Iryna (Irka_do)</p>
             """;

        await _emailSender.SendAsync(
            purchase.CustomerEmail,
            $"Your guide is ready: {purchase.TravelGuide!.Title}",
            body,
            cancellationToken);

        purchase.DeliveredAt = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Delivered guide {GuideId} for purchase {PurchaseId}.", purchase.TravelGuideId, purchaseId);
    }
}
