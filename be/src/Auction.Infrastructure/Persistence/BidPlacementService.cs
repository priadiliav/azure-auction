using Auction.Application.Items;
using Auction.Application.Items.PlaceBid;
using Auction.Domain.Bids;
using Microsoft.EntityFrameworkCore;

namespace Auction.Infrastructure.Persistence;

public class BidPlacementService(AuctionDbContext dbContext) : IBidPlacementService
{
    private const int MaxConcurrencyRetries = 3;

    public async Task<PlaceBidResult> PlaceBidAsync(
        Guid itemId, decimal amount, string bidderId, CancellationToken cancellationToken)
    {
        for (var attempt = 0; attempt < MaxConcurrencyRetries; attempt++)
        {
            var item = await dbContext.Items.SingleOrDefaultAsync(i => i.Id == itemId, cancellationToken);
            if (item is null)
            {
                return new PlaceBidResult(false, PlaceBidFailureReason.ItemNotFound, 0);
            }

            if (DateTimeOffset.UtcNow >= item.EndsAt)
            {
                return new PlaceBidResult(false, PlaceBidFailureReason.AuctionEnded, item.CurrentPrice);
            }

            if (amount <= item.CurrentPrice)
            {
                return new PlaceBidResult(false, PlaceBidFailureReason.BidTooLow, item.CurrentPrice);
            }

            item.PlaceBid(amount);
            dbContext.Bids.Add(new Bid(Guid.CreateVersion7(), itemId, amount, bidderId, DateTimeOffset.UtcNow));

            try
            {
                // Item.RowVersion is a concurrency token - this throws if another bid was
                // committed for the same item since it was read above, instead of silently
                // overwriting it.
                await dbContext.SaveChangesAsync(cancellationToken);
                return new PlaceBidResult(true, PlaceBidFailureReason.None, amount);
            }
            catch (DbUpdateConcurrencyException)
            {
                dbContext.ChangeTracker.Clear();
            }
        }

        return new PlaceBidResult(false, PlaceBidFailureReason.BidTooLow, 0);
    }
}
