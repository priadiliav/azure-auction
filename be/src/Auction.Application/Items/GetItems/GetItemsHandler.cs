using Auction.Application.Items.GetItem;
using MediatR;

namespace Auction.Application.Items.GetItems;

public class GetItemsHandler(IItemRepository itemRepository, IBidRepository bidRepository)
    : IRequestHandler<GetItemsQuery, IReadOnlyList<GetItemResult>>
{
    private const int RecentBidCount = 3;

    public async Task<IReadOnlyList<GetItemResult>> Handle(GetItemsQuery request, CancellationToken cancellationToken)
    {
        var items = request.SellerId is null
            ? await itemRepository.GetAllAsync(cancellationToken)
            : await itemRepository.GetBySellerIdAsync(request.SellerId, cancellationToken);

        // todo: here we will do filtering, sorting, and pagination in the future + recomendations

        var results = new List<GetItemResult>(items.Count);
        foreach (var item in items)
        {
            var recentBids = await bidRepository.GetRecentBidsAsync(item.Id, RecentBidCount, cancellationToken);
            results.Add(GetItemResult.FromItem(item, recentBids));
        }

        return results;
    }
}
