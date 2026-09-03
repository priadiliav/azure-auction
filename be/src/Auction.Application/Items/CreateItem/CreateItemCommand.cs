using MediatR;

namespace Auction.Application.Items.CreateItem;

public record CreateItemCommand(
    string Title,
    string Description,
    decimal StartingPrice,
    string SellerId) : IRequest<Guid>;
