namespace Auction.Application.Items;

public record RecentBid(string BidderId, string BidderName, string BidderAvatarUrl, decimal Amount, DateTimeOffset PlacedAt);

public interface IBidRepository
{
    Task<IReadOnlyList<RecentBid>> GetRecentBidsAsync(Guid itemId, int count, CancellationToken cancellationToken);
}
