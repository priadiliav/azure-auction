using Auction.Contracts;
using Auction.Domain.Items;
using MediatR;

namespace Auction.Application.Items.CreateItem;

public class CreateItemHandler(IItemRepository itemRepository, IItemPublisher itemPublisher)
    : IRequestHandler<CreateItemCommand, Guid>
{
    public async Task<Guid> Handle(CreateItemCommand request, CancellationToken cancellationToken)
    {
        var itemId = Guid.CreateVersion7();
        var item = new Item(itemId, request.Title, request.StartingPrice);

        // use outbox pattern instead
        await itemRepository.AddAsync(item, cancellationToken);
        await itemPublisher.PublishAsync(
            new ItemCreatedMessage(itemId, request.Title, request.StartingPrice),
            cancellationToken);

        return itemId;
    }
}
