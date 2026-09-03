namespace Auction.Application.Items;

public record BidPlacedMessage(Guid ItemId, decimal Amount, string BidderId, string BidderName, string BidderAvatarUrl);

public interface IBidPublisher
{
    Task PublishAsync(BidPlacedMessage message, CancellationToken cancellationToken);
}
