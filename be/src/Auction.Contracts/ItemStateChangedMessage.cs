namespace Auction.Contracts;

public record ItemStateChangedMessage(Guid ItemId, string Status);
