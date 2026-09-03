namespace Auction.Domain.Bids;

public class Bid
{
    public Guid Id { get; private set; }
    public Guid ItemId { get; private set; }
    public decimal Amount { get; private set; }
    public string BidderName { get; private set; } = string.Empty;
    public DateTimeOffset PlacedAt { get; private set; }

    private Bid()
    {
    }

    public Bid(Guid id, Guid itemId, decimal amount, string bidderName, DateTimeOffset placedAt)
    {
        Id = id;
        ItemId = itemId;
        Amount = amount;
        BidderName = bidderName;
        PlacedAt = placedAt;
    }
}
