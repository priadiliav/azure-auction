namespace Auction.Application.Items.GetItem;

public record GetItemResult(
    Guid ItemId,
    string Title,
    decimal StartingPrice,
    decimal CurrentPrice,
    string Status,
    string? BlobUrl,
    DateTimeOffset EndsAt);
