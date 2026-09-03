using Auction.Application.Users;
using MediatR;

namespace Auction.Application.Items.PlaceBid;

public class PlaceBidHandler(
    IBidPlacementService bidPlacementService,
    IBidPublisher bidPublisher,
    IUserRepository userRepository)
    : IRequestHandler<PlaceBidCommand, PlaceBidResult>
{
    public async Task<PlaceBidResult> Handle(PlaceBidCommand request, CancellationToken cancellationToken)
    {
        var result = await bidPlacementService.PlaceBidAsync(
            request.ItemId, request.Amount, request.BidderId, cancellationToken);

        if (result.Accepted)
        {
            var bidder = await userRepository.GetByIdAsync(request.BidderId, cancellationToken);
            await bidPublisher.PublishAsync(
                new BidPlacedMessage(
                    request.ItemId, request.Amount, request.BidderId,
                    bidder?.Name ?? "Unknown", bidder?.AvatarUrl ?? string.Empty),
                cancellationToken);
        }

        return result;
    }
}
