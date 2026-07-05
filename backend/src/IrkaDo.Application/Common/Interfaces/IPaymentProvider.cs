namespace IrkaDo.Application.Common.Interfaces;

public record CheckoutSessionRequest(
    Guid TravelGuideId,
    string GuideTitle,
    decimal Amount,
    string Currency,
    string SuccessUrl,
    string CancelUrl);

public record CheckoutSessionResult(string SessionId, string CheckoutUrl);

public interface IPaymentProvider
{
    Task<CheckoutSessionResult> CreateCheckoutSessionAsync(
        CheckoutSessionRequest request, CancellationToken cancellationToken = default);

    /// <summary>Validates and parses a provider webhook payload into a provider-agnostic result.</summary>
    Task<PaymentWebhookEvent> ParseWebhookAsync(
        string payload, string signatureHeader, CancellationToken cancellationToken = default);
}

public enum PaymentWebhookEventType
{
    Unknown,
    CheckoutCompleted,
    PaymentFailed
}

public record PaymentWebhookEvent(
    PaymentWebhookEventType Type,
    string SessionId,
    string? PaymentId,
    string? CustomerEmail);
