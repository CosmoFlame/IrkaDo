namespace IrkaDo.Application.Common.Interfaces;

/// <summary>
/// Delivers a completed purchase: generates a signed download link for the guide file and emails it
/// to the buyer. Idempotent — a purchase that has already been delivered is skipped.
/// </summary>
public interface IGuideDeliveryService
{
    Task DeliverAsync(Guid purchaseId, CancellationToken cancellationToken = default);
}
