namespace Auction.Domain.Items;

public class Item
{
    private static readonly TimeSpan AuctionDuration = TimeSpan.FromHours(24);

    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public decimal StartingPrice { get; private set; }
    public decimal CurrentPrice { get; private set; }
    public ItemStatus Status { get; private set; }
    public string? BlobUrl { get; private set; }
    public DateTimeOffset EndsAt { get; private set; }
    public byte[] RowVersion { get; private set; } = [];

    private Item()
    {
    }

    public Item(Guid id, string title, decimal startingPrice)
    {
        Id = id;
        Title = title;
        StartingPrice = startingPrice;
        CurrentPrice = startingPrice;
        Status = ItemStatus.New;
        EndsAt = DateTimeOffset.UtcNow.Add(AuctionDuration);
    }

    public void MarkInProcess() => Status = ItemStatus.InProcess;
    public void MarkAnalysis() => Status = ItemStatus.Analysis;
    public void MarkListed() => Status = ItemStatus.Listed;
    public void MarkFailed() => Status = ItemStatus.Failed;

    public void SetBlobUrl(string blobUrl) => BlobUrl = blobUrl;

    public void PlaceBid(decimal amount) => CurrentPrice = amount;

    public void Close() => Status = ItemStatus.Ended;
}
