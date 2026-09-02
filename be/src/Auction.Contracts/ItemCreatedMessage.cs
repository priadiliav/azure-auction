namespace Auction.Contracts;

public record ItemCreatedMessage(Guid ItemId, string Title, decimal StartingPrice);
