using Auction.Application.Items.PlaceBid;

namespace Auction.Application.Items;

public interface IBidPlacementService
{
    /// <summary>
    /// Validates and records a bid against an item atomically, retrying on a concurrent bid conflict.
    /// </summary>
    Task<PlaceBidResult> PlaceBidAsync(Guid itemId, decimal amount, string bidderId, CancellationToken cancellationToken);
}
