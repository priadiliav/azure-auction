using Auction.Application.Items.CreateItem;
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
    }
}
