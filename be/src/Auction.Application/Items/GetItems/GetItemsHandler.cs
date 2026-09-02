using Auction.Application.Items.GetItem;
using MediatR;

namespace Auction.Application.Items.GetItems;

public class GetItemsHandler(IItemRepository itemRepository) : IRequestHandler<GetItemsQuery, IReadOnlyList<GetItemResult>>
{
    public async Task<IReadOnlyList<GetItemResult>> Handle(GetItemsQuery request, CancellationToken cancellationToken)
    {
        var items = await itemRepository.GetAllAsync(cancellationToken);
    
        // todo: use pagination instead.
        return items
            .Select(item => new GetItemResult(item.Id, item.Title, item.StartingPrice, item.Status.ToString()))
            .ToList();
    }
}
