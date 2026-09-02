using MediatR;

namespace Auction.Application.Items.GetItem;

public record GetItemQuery(Guid ItemId) : IRequest<GetItemResult?>;
