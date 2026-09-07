using Auction.Application.Items.GetItem;
using Auction.Domain.Items;
using MediatR;

namespace Auction.Application.Items.GetItems;

public class GetItemsHandler(
    IItemRepository itemRepository,
    IBidRepository bidRepository,
    IInteractionRepository interactionRepository)
    : IRequestHandler<GetItemsQuery, IReadOnlyList<GetItemResult>>
{
    private const int RecentBidCount = 3;

    public async Task<IReadOnlyList<GetItemResult>> Handle(GetItemsQuery request, CancellationToken cancellationToken)
    {
        var items = request.SellerId is null
            ? await itemRepository.GetAllAsync(cancellationToken)
            : await itemRepository.GetBySellerIdAsync(request.SellerId, cancellationToken);

        var orderedItems = await OrderByRecommendationAsync(items, request, cancellationToken);

        var results = new List<GetItemResult>(orderedItems.Count);
        foreach (var item in orderedItems)
        {
            var recentBids = await bidRepository.GetRecentBidsAsync(item.Id, RecentBidCount, cancellationToken);
            results.Add(GetItemResult.FromItem(item, recentBids));
        }

        return results;
    }

    private async Task<IReadOnlyList<Item>> OrderByRecommendationAsync(
        IReadOnlyList<Item> items, GetItemsQuery request, CancellationToken cancellationToken)
    {
        if (request.SellerId is not null || request.RequestingUserId is null)
        {
            return items;
        }

        var tasteVector = await interactionRepository.GetUserTasteVectorAsync(request.RequestingUserId, cancellationToken);
        if (tasteVector is null)
        {
            return items;
        }

        return items
            .Select(item => (Item: item, Score: item.Embedding is null
                ? double.NegativeInfinity
                : EmbeddingVector.Cosine(tasteVector, EmbeddingVector.Parse(item.Embedding))))
            .OrderByDescending(x => x.Score)
            .Select(x => x.Item)
            .ToList();
    }
}
