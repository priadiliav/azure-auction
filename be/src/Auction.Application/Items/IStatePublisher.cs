namespace Auction.Application.Items;

public record ItemStateChangedMessage(Guid ItemId, string Status);

public interface IStatePublisher
{
    /// <summary>
    /// Publish an item state change
    /// </summary>
    Task PublishAsync(ItemStateChangedMessage message, CancellationToken cancellationToken);
}
