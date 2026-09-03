using MediatR;

namespace Auction.Application.Items.CreateItem;

public record CreateItemCommand(
    string Title, 
    decimal StartingPrice) : IRequest<Guid>;
