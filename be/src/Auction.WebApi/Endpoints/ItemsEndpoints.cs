using System.Security.Claims;
using Auction.Application.Items.CreateItem;
using Auction.Application.Items.GetItem;
using Auction.Application.Items.GetItems;
using Auction.Application.Items.PlaceBid;
using MediatR;

namespace Auction.WebApi.Endpoints;

public static class ItemsEndpoints
{
    public static void MapItemsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/items", async (CreateItemRequest request, ClaimsPrincipal user, IMediator mediator) =>
        {
            var sellerId = user.FindFirstValue("sub") ?? user.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var itemId = await mediator.Send(
                new CreateItemCommand(request.Title, request.Description, request.StartingPrice, sellerId));
            return Results.Ok(new { itemId });
        }).RequireAuthorization();

        app.MapGet("/api/items", async (ClaimsPrincipal user, IMediator mediator) =>
        {
            var requestingUserId = user.Identity?.IsAuthenticated == true
                ? user.FindFirstValue("sub") ?? user.FindFirstValue(ClaimTypes.NameIdentifier)
                : null;
            var results = await mediator.Send(new GetItemsQuery(RequestingUserId: requestingUserId));
            return Results.Ok(results);
        });

        app.MapGet("/api/items/mine", async (ClaimsPrincipal user, IMediator mediator) =>
        {
            var sellerId = user.FindFirstValue("sub") ?? user.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var results = await mediator.Send(new GetItemsQuery(sellerId));
            return Results.Ok(results);
        }).RequireAuthorization();

        app.MapGet("/api/items/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetItemQuery(id));
            return result is null
                ? Results.NotFound()
                : Results.Ok(result);
        });

        app.MapPost("/api/items/{id:guid}/bids", async (Guid id, PlaceBidRequest request, ClaimsPrincipal user, IMediator mediator) =>
        {
            var bidderId = user.FindFirstValue("sub") ?? user.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await mediator.Send(new PlaceBidCommand(id, request.Amount, bidderId));

            return result.Reason switch
            {
                PlaceBidFailureReason.ItemNotFound => Results.NotFound(),
                PlaceBidFailureReason.AuctionEnded => Results.Conflict(new { reason = "AuctionEnded", currentPrice = result.CurrentPrice }),
                PlaceBidFailureReason.BidTooLow => Results.Conflict(new { reason = "BidTooLow", currentPrice = result.CurrentPrice }),
                _ => Results.Ok(new { currentPrice = result.CurrentPrice }),
            };
        }).RequireAuthorization();
    }
}

public record CreateItemRequest(string Title, string Description, decimal StartingPrice);

public record PlaceBidRequest(decimal Amount);
