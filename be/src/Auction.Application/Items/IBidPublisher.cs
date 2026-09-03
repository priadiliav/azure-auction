namespace Auction.Application.Items;

public record BidPlacedMessage(Guid ItemId, decimal Amount, string BidderName);

public interface IBidPublisher
{
    Task PublishAsync(BidPlacedMessage message, CancellationToken cancellationToken);
}
