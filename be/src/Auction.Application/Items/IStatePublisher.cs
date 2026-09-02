using Auction.Contracts;

namespace Auction.Application.Items;

public interface IStatePublisher
{
    /// <summary>
    /// Publish an item state change
    /// </summary>
    Task PublishAsync(ItemStateChangedMessage message, CancellationToken cancellationToken);
}
