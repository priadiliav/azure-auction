using Auction.Contracts;
using MediatR;

namespace Auction.Application.Items.CreateItem;

public class CreateItemHandler(IItemEventPublisher itemEventPublisher) : IRequestHandler<CreateItemCommand, Guid>
{
    public async Task<Guid> Handle(CreateItemCommand request, CancellationToken cancellationToken)
    {
        var itemId = Guid.CreateVersion7();

        await itemEventPublisher.PublishItemCreatedAsync(
            new ItemCreatedMessage(itemId, request.Title, request.StartingPrice),
            cancellationToken);

        return itemId;
    }
}
