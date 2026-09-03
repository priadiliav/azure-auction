namespace Auction.Application.Items;

public record ItemCreatedMessage(Guid ItemId);

public interface IItemPublisher
{
    /// <summary>
    /// Publish created item
    /// </summary>
    Task PublishAsync(ItemCreatedMessage message, CancellationToken cancellationToken);
}
