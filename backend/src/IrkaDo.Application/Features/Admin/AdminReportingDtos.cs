using IrkaDo.Domain.Entities;

namespace IrkaDo.Application.Features.Admin;

// --- Purchases (read-only reporting) ---

public record AdminPurchaseDto(
    Guid Id,
    string? GuideTitle,
    string CustomerEmail,
    decimal AmountPaid,
    string Currency,
    PurchaseStatus Status,
    string PaymentProvider,
    string ProviderSessionId,
    DateTimeOffset? DeliveredAt,
    DateTimeOffset CreatedAt);

// --- Download logs (read-only reporting) ---

public record AdminDownloadLogDto(
    Guid Id,
    string? GuideTitle,
    string? Email,
    string IpAddress,
    DateTimeOffset CreatedAt);

// --- Dashboard summary ---

public record AdminDashboardDto(
    int NewsTotal,
    int NewsPublished,
    int GuidesTotal,
    int GuidesPublished,
    int PremiumGuides,
    int Collaborations,
    int SocialLinks,
    int Highlights,
    int MediaAssets,
    int PurchasesCompleted,
    decimal Revenue,
    int DownloadsTotal);
