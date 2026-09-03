using Auction.Application.Items;
using Microsoft.EntityFrameworkCore;

namespace Auction.Infrastructure.Persistence;

public class BidRepository(AuctionDbContext dbContext) : IBidRepository
{
    public async Task<IReadOnlyList<RecentBid>> GetRecentBidsAsync(Guid itemId, int count, CancellationToken cancellationToken)
        => await dbContext.Bids
            .Where(bid => bid.ItemId == itemId)
            .OrderByDescending(bid => bid.PlacedAt)
            .Take(count)
            .Join(
                dbContext.Users,
                bid => bid.BidderId,
                user => user.Id,
                (bid, user) => new RecentBid(user.Id, user.Name, user.AvatarUrl, bid.Amount, bid.PlacedAt))
            .ToListAsync(cancellationToken);
}
