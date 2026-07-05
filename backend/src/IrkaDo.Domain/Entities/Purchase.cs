using IrkaDo.Domain.Common;

namespace IrkaDo.Domain.Entities;

public enum PurchaseStatus
{
    Pending,
    Completed,
    Failed
}

public class Purchase : BaseEntity
{
    public Guid TravelGuideId { get; set; }
    public TravelGuide? TravelGuide { get; set; }

    public string CustomerEmail { get; set; } = string.Empty;
    public decimal AmountPaid { get; set; }
    public string Currency { get; set; } = "USD";
    public PurchaseStatus Status { get; set; } = PurchaseStatus.Pending;

    public string PaymentProvider { get; set; } = "Stripe";
    public string ProviderSessionId { get; set; } = string.Empty;
    public string? ProviderPaymentId { get; set; }

    public DateTimeOffset? DeliveredAt { get; set; }
}
