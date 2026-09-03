using System.Security.Claims;
using Auction.Application.Users.GetMe;
using Auction.Infrastructure.Auth;
using MediatR;

namespace Auction.WebApi.Endpoints;

public static class MeEndpoints
{
    public static void MapMeEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/me", async (ClaimsPrincipal principal, IMediator mediator) =>
        {
            var (id, _, _, _) = GoogleTokenValidation.ExtractUserProfile(principal);
            var result = await mediator.Send(new GetMeQuery(id));
            return result is null
                ? Results.NotFound()
                : Results.Ok(result);
        }).RequireAuthorization();
    }
}
