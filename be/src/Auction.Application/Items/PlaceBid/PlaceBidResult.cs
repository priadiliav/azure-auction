namespace Auction.Application.Items.PlaceBid;

public enum PlaceBidFailureReason
{
    None,
    ItemNotFound,
    BidTooLow,
    AuctionEnded,
}

public record PlaceBidResult(bool Accepted, PlaceBidFailureReason Reason, decimal CurrentPrice);
