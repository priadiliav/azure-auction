using Auction.Application.Items.GetItem;
using MediatR;

namespace Auction.Application.Items.GetItems;

public record GetItemsQuery(string? SellerId = null, string? RequestingUserId = null) : IRequest<IReadOnlyList<GetItemResult>>;
