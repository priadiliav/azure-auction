namespace Auction.Domain.Items;

public class Item
{
    private static readonly TimeSpan AuctionDuration = TimeSpan.FromHours(24);

    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal StartingPrice { get; private set; }
    public decimal CurrentPrice { get; private set; }
    public ItemStatus Status { get; private set; }
    public string? BlobUrl { get; private set; }
    public DateTimeOffset EndsAt { get; private set; }
    public string SellerId { get; private set; } = string.Empty;
    public byte[] RowVersion { get; private set; } = [];

    private Item()
    {
    }

    public Item(Guid id, string title, string description, decimal startingPrice, string sellerId)
    {
        Id = id;
        Title = title;
        Description = description;
        StartingPrice = startingPrice;
        CurrentPrice = startingPrice;
        Status = ItemStatus.New;
        EndsAt = DateTimeOffset.UtcNow.Add(AuctionDuration);
        SellerId = sellerId;
    }

    public void MarkInProcess() => Status = ItemStatus.InProcess;
    public void MarkAnalysis() => Status = ItemStatus.Analysis;
    public void MarkListed() => Status = ItemStatus.Listed;
    public void MarkFailed() => Status = ItemStatus.Failed;

    public void SetBlobUrl(string blobUrl) => BlobUrl = blobUrl;

    public void PlaceBid(decimal amount) => CurrentPrice = amount;

    public void Close() => Status = ItemStatus.Ended;
}
