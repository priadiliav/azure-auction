namespace Auction.Application.Items.GetItem;

public record GetItemResult(Guid ItemId, string Title, decimal StartingPrice, string Status);
