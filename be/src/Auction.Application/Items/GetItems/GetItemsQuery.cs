using Auction.Application.Items.GetItem;
using MediatR;

namespace Auction.Application.Items.GetItems;

public record GetItemsQuery : IRequest<IReadOnlyList<GetItemResult>>;
