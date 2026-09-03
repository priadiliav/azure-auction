using MediatR;

namespace Auction.Application.Items.PlaceBid;

public record PlaceBidCommand(Guid ItemId, decimal Amount, string BidderName) : IRequest<PlaceBidResult>;
