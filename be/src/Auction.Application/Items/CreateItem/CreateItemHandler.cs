using Auction.Domain.Items;
using MediatR;

namespace Auction.Application.Items.CreateItem;

public class CreateItemHandler(IItemRepository itemRepository)
    : IRequestHandler<CreateItemCommand, Guid>
{
    public async Task<Guid> Handle(CreateItemCommand request, CancellationToken cancellationToken)
    {
        var itemId = Guid.CreateVersion7();
        var item = new Item(itemId, request.Title, request.StartingPrice);
        await itemRepository.AddAsync(item, cancellationToken);
        return itemId;
    }
}
