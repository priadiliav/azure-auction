using MediatR;

namespace Auction.Application.Items.GetItem;

public class GetItemHandler(IItemRepository itemRepository) : IRequestHandler<GetItemQuery, GetItemResult?>
{
    public async Task<GetItemResult?> Handle(GetItemQuery request, CancellationToken cancellationToken)
    {
        var item = await itemRepository.GetByIdAsync(request.ItemId, cancellationToken);

        return item is null
            ? null
            : new GetItemResult(
                item.Id, 
                item.Title, 
                item.StartingPrice, 
                item.Status.ToString(),
                item.BlobUrl);
    }
}
