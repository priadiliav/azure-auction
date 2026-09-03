using Auction.Application.Items.GetItem;
using MediatR;

namespace Auction.Application.Items.GetItems;

public class GetItemsHandler(IItemRepository itemRepository) : IRequestHandler<GetItemsQuery, IReadOnlyList<GetItemResult>>
{
    public async Task<IReadOnlyList<GetItemResult>> Handle(GetItemsQuery request, CancellationToken cancellationToken)
    {
        var items = await itemRepository.GetAllAsync(cancellationToken);
    
        // todo: here we will do filtering, sorting, and pagination in the future + recomendations
        
        return items
            .Select(item => new GetItemResult(
                item.Id, 
                item.Title, 
                item.StartingPrice, 
                item.CurrentPrice, 
                item.Status.ToString(), 
                item.BlobUrl, 
                item.EndsAt))
            .ToList();
    }
}
