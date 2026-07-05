using IrkaDo.Application.Common.Interfaces;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace IrkaDo.Infrastructure.Payments;

public class StripeOptions
{
    public string SecretKey { get; set; } = string.Empty;
    public string WebhookSecret { get; set; } = string.Empty;
}

public class StripePaymentProvider : IPaymentProvider
{
    private readonly StripeOptions _options;

    public StripePaymentProvider(IOptions<StripeOptions> options)
    {
        _options = options.Value;
        StripeConfiguration.ApiKey = _options.SecretKey;
    }

    public async Task<CheckoutSessionResult> CreateCheckoutSessionAsync(
        CheckoutSessionRequest request, CancellationToken cancellationToken = default)
    {
        var service = new SessionService();
        var session = await service.CreateAsync(new SessionCreateOptions
        {
            Mode = "payment",
            SuccessUrl = request.SuccessUrl,
            CancelUrl = request.CancelUrl,
            CustomerEmail = null,
            LineItems =
            [
                new SessionLineItemOptions
                {
                    Quantity = 1,
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = request.Currency,
                        UnitAmount = (long)(request.Amount * 100),
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = request.GuideTitle
                        }
                    }
                }
            ],
            Metadata = new Dictionary<string, string>
            {
                ["travelGuideId"] = request.TravelGuideId.ToString()
            }
        }, cancellationToken: cancellationToken);

        return new CheckoutSessionResult(session.Id, session.Url);
    }

    public Task<PaymentWebhookEvent> ParseWebhookAsync(
        string payload, string signatureHeader, CancellationToken cancellationToken = default)
    {
        var stripeEvent = EventUtility.ConstructEvent(payload, signatureHeader, _options.WebhookSecret);

        if (stripeEvent.Data.Object is Session session)
        {
            var type = stripeEvent.Type switch
            {
                "checkout.session.completed" => PaymentWebhookEventType.CheckoutCompleted,
                "checkout.session.async_payment_failed" => PaymentWebhookEventType.PaymentFailed,
                _ => PaymentWebhookEventType.Unknown
            };

            return Task.FromResult(new PaymentWebhookEvent(
                type, session.Id, session.PaymentIntentId, session.CustomerEmail));
        }

        return Task.FromResult(new PaymentWebhookEvent(PaymentWebhookEventType.Unknown, string.Empty, null, null));
    }
}
