using Auction.Domain.Items;

namespace Auction.Application.Items.GetItem;

public record GetItemResult(
    Guid ItemId,
    string Title,
    string Description,
    decimal StartingPrice,
    decimal CurrentPrice,
    string Status,
    string? BlobUrl,
    DateTimeOffset EndsAt,
    string SellerId,
    IReadOnlyList<RecentBid> RecentBids)
{
    public static GetItemResult FromItem(Item item, IReadOnlyList<RecentBid> recentBids) => new(
        item.Id,
        item.Title,
        item.Description,
        item.StartingPrice,
        item.CurrentPrice,
        item.Status.ToString(),
        item.BlobUrl,
        item.EndsAt,
        item.SellerId,
        recentBids);
}
