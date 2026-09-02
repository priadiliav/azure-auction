using Auction.Contracts;

namespace Auction.Application.Items;

public interface IItemPublisher
{
    /// <summary>
    /// Publish created item
    /// </summary>
    Task PublishAsync(ItemCreatedMessage message, CancellationToken cancellationToken);
}
