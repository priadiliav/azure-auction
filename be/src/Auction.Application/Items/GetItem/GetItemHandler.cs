using MediatR;

namespace Auction.Application.Items.GetItem;

public class GetItemHandler(IItemRepository itemRepository, IBidRepository bidRepository)
    : IRequestHandler<GetItemQuery, GetItemResult?>
{
    private const int RecentBidCount = 3;

    public async Task<GetItemResult?> Handle(GetItemQuery request, CancellationToken cancellationToken)
    {
        var item = await itemRepository.GetByIdAsync(request.ItemId, cancellationToken);
        if (item is null)
        {
            return null;
        }

        var recentBids = await bidRepository.GetRecentBidsAsync(item.Id, RecentBidCount, cancellationToken);
        return GetItemResult.FromItem(item, recentBids);
    }
}
