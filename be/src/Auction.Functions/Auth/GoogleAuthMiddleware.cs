using System.IdentityModel.Tokens.Jwt;
using System.Net;
using Auction.Functions.Functions;
using Auction.Infrastructure.Auth;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Configuration;

namespace Auction.Functions.Auth;

/// <summary>
/// Validates a Google ID token on protected HTTP-triggered functions. Unlike WebApi, the isolated
/// worker model doesn't expose the ASP.NET Core auth middleware pipeline, so this validates the
/// token manually and short-circuits with 401 on failure.
/// </summary>
public class GoogleAuthMiddleware(IConfiguration configuration) : IFunctionsWorkerMiddleware
{
    private static readonly HashSet<string> ProtectedFunctions = new(StringComparer.OrdinalIgnoreCase)
    {
        nameof(BlobSasHttpTrigger),
    };

    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        if (!ProtectedFunctions.Contains(context.FunctionDefinition.Name))
        {
            await next(context);
            return;
        }

        var req = await context.GetHttpRequestDataAsync();
        if (req is null)
        {
            await next(context);
            return;
        }

        if (!TryGetBearerToken(req, out var token))
        {
            context.GetInvocationResult().Value = req.CreateResponse(HttpStatusCode.Unauthorized);
            return;
        }

        var clientId = configuration["Authentication:Google:ClientId"]
            ?? throw new InvalidOperationException("Missing configuration: Authentication:Google:ClientId");

        try
        {
            var validationParameters = await GoogleTokenValidation.CreateValidationParametersAsync(
                clientId, context.CancellationToken);
            new JwtSecurityTokenHandler().ValidateToken(token, validationParameters, out _);
        }
        catch
        {
            context.GetInvocationResult().Value = req.CreateResponse(HttpStatusCode.Unauthorized);
            return;
        }

        await next(context);
    }

    private static bool TryGetBearerToken(HttpRequestData req, out string token)
    {
        token = string.Empty;
        if (!req.Headers.TryGetValues("Authorization", out var values))
        {
            return false;
        }

        var header = values.FirstOrDefault();
        if (header is null || !header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        token = header["Bearer ".Length..];
        return !string.IsNullOrWhiteSpace(token);
    }
}
