using IrkaDo.Application.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IrkaDo.Infrastructure.Payments;

/// <summary>
/// Drains the <see cref="GuideDeliveryQueue"/> and delivers each purchase in its own DI scope,
/// retrying transient failures (e.g. the email provider being briefly unavailable) with a short
/// backoff. Because <see cref="IGuideDeliveryService"/> is idempotent, a retry never double-sends.
/// </summary>
public class GuideDeliveryBackgroundService : BackgroundService
{
    private const int MaxAttempts = 4;

    private readonly GuideDeliveryQueue _queue;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<GuideDeliveryBackgroundService> _logger;

    public GuideDeliveryBackgroundService(
        GuideDeliveryQueue queue,
        IServiceScopeFactory scopeFactory,
        ILogger<GuideDeliveryBackgroundService> logger)
    {
        _queue = queue;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await foreach (var purchaseId in _queue.ReadAllAsync(stoppingToken))
            {
                await DeliverWithRetryAsync(purchaseId, stoppingToken);
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
        }
    }

    private async Task DeliverWithRetryAsync(Guid purchaseId, CancellationToken stoppingToken)
    {
        for (var attempt = 1; attempt <= MaxAttempts; attempt++)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var delivery = scope.ServiceProvider.GetRequiredService<IGuideDeliveryService>();
                await delivery.DeliverAsync(purchaseId, stoppingToken);
                return;
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex, "Guide delivery attempt {Attempt}/{Max} failed for purchase {PurchaseId}.",
                    attempt, MaxAttempts, purchaseId);

                if (attempt == MaxAttempts)
                {
                    _logger.LogError(
                        ex, "Guide delivery permanently failed for purchase {PurchaseId} after {Max} attempts.",
                        purchaseId, MaxAttempts);
                    return;
                }

                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(5 * attempt), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    return;
                }
            }
        }
    }
}
