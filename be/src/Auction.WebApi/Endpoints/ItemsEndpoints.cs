using Auction.Application.Items.CreateItem;
using Auction.Application.Items.GetItem;
using Auction.Application.Items.GetItems;
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
    }
}
