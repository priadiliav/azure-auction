namespace Auction.Domain.Items;

public class Item
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public decimal StartingPrice { get; private set; }
    public ItemStatus Status { get; private set; }
    public string? BlobUrl { get; private set; }

    private Item()
    {
    }

    public Item(Guid id, string title, decimal startingPrice)
    {
        Id = id;
        Title = title;
        StartingPrice = startingPrice;
        Status = ItemStatus.New;
    }

    public void MarkInProcess() => Status = ItemStatus.InProcess;
    public void MarkAnalysis() => Status = ItemStatus.Analysis;
    public void MarkListed() => Status = ItemStatus.Listed;
    public void MarkFailed() => Status = ItemStatus.Failed;
    public void SetBlobUrl(string blobUrl) => BlobUrl = blobUrl;
}
