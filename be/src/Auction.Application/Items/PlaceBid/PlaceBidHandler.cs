using MediatR;

namespace Auction.Application.Items.PlaceBid;

public class PlaceBidHandler(IBidPlacementService bidPlacementService, IBidPublisher bidPublisher)
    : IRequestHandler<PlaceBidCommand, PlaceBidResult>
{
    public async Task<PlaceBidResult> Handle(PlaceBidCommand request, CancellationToken cancellationToken)
    {
        var result = await bidPlacementService.PlaceBidAsync(
            request.ItemId, request.Amount, request.BidderName, cancellationToken);

        if (result.Accepted)
        {
            await bidPublisher.PublishAsync(
                new BidPlacedMessage(request.ItemId, request.Amount, request.BidderName),
                cancellationToken);
        }

        return result;
    }
}
