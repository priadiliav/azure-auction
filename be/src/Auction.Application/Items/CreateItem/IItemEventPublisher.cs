using Auction.Contracts;

namespace Auction.Application.Items.CreateItem;

public interface IItemEventPublisher
{
    Task PublishItemCreatedAsync(ItemCreatedMessage message, CancellationToken cancellationToken);
}
