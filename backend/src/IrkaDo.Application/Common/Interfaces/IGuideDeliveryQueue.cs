namespace IrkaDo.Application.Common.Interfaces;

/// <summary>
/// Hands off a completed purchase for asynchronous guide delivery (signed download link + email)
/// so the payment webhook can respond immediately and delivery failures never block the response.
/// </summary>
public interface IGuideDeliveryQueue
{
    void Enqueue(Guid purchaseId);
}
