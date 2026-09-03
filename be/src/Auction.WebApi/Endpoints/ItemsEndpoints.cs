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
        app.MapPost("/api/items", async (CreateItemCommand command, IMediator mediator) =>
        {
            var itemId = await mediator.Send(command);
            return Results.Ok(new { itemId });
        });

        app.MapGet("/api/items", async (IMediator mediator) =>
        {
            var results = await mediator.Send(new GetItemsQuery());
            return Results.Ok(results);
        });

        app.MapGet("/api/items/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetItemQuery(id));
            return result is null
                ? Results.NotFound()
                : Results.Ok(result);
        });

        app.MapPost("/api/items/{id:guid}/bids", async (Guid id, PlaceBidRequest request, IMediator mediator) =>
        {
            var result = await mediator.Send(new PlaceBidCommand(id, request.Amount, request.BidderName));

            return result.Reason switch
            {
                PlaceBidFailureReason.ItemNotFound => Results.NotFound(),
                PlaceBidFailureReason.AuctionEnded => Results.Conflict(new { reason = "AuctionEnded", currentPrice = result.CurrentPrice }),
                PlaceBidFailureReason.BidTooLow => Results.Conflict(new { reason = "BidTooLow", currentPrice = result.CurrentPrice }),
                _ => Results.Ok(new { currentPrice = result.CurrentPrice }),
            };
        });
    }
}

public record PlaceBidRequest(decimal Amount, string BidderName);
