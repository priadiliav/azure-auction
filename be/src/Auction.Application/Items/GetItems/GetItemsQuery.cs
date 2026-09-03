using Auction.Application.Items.GetItem;
using MediatR;

namespace Auction.Application.Items.GetItems;

public record GetItemsQuery(string? SellerId = null) : IRequest<IReadOnlyList<GetItemResult>>;
