using System.Threading.Channels;
using IrkaDo.Application.Common.Interfaces;

namespace IrkaDo.Infrastructure.Payments;

/// <summary>
/// In-process, unbounded queue of purchase ids awaiting delivery, backed by a <see cref="Channel{T}"/>.
/// Registered as a singleton so the payment webhook (scoped) and the delivery background service
/// (singleton) share the same channel. A simple in-memory queue is sufficient for the expected
/// low-to-moderate volume; it can be swapped for a durable queue behind the same interface later.
/// </summary>
public class GuideDeliveryQueue : IGuideDeliveryQueue
{
    private readonly Channel<Guid> _channel = Channel.CreateUnbounded<Guid>(
        new UnboundedChannelOptions { SingleReader = true });

    public void Enqueue(Guid purchaseId) => _channel.Writer.TryWrite(purchaseId);

    public IAsyncEnumerable<Guid> ReadAllAsync(CancellationToken cancellationToken) =>
        _channel.Reader.ReadAllAsync(cancellationToken);
}
