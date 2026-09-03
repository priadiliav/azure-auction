using System.Security.Claims;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace Auction.Infrastructure.Auth;

public static class GoogleTokenValidation
{
    public const string Issuer = "https://accounts.google.com";

    private const string DiscoveryDocumentUrl = "https://accounts.google.com/.well-known/openid-configuration";

    private static readonly ConfigurationManager<OpenIdConnectConfiguration> ConfigurationManager =
        new(DiscoveryDocumentUrl, new OpenIdConnectConfigurationRetriever());

    /// <summary>
    /// Builds validation parameters for a Google-issued ID token, fetching Google's current signing
    /// keys from its OIDC discovery document (cached internally, refreshed automatically).
    /// </summary>
    public static async Task<TokenValidationParameters> CreateValidationParametersAsync(
        string clientId, CancellationToken cancellationToken)
    {
        var config = await ConfigurationManager.GetConfigurationAsync(cancellationToken);

        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = Issuer,
            ValidateAudience = true,
            ValidAudience = clientId,
            ValidateLifetime = true,
            IssuerSigningKeys = config.SigningKeys,
        };
    }

    /// <summary>
    /// Pulls the Google profile fields we care about off a validated token's claims.
    /// </summary>
    public static (string Id, string Email, string Name, string AvatarUrl) ExtractUserProfile(ClaimsPrincipal principal)
    {
        var id = FindClaim(principal, "sub", ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("Token is missing a 'sub' claim.");
        var email = FindClaim(principal, "email", ClaimTypes.Email) ?? string.Empty;
        var name = FindClaim(principal, "name") ?? string.Empty;
        var avatarUrl = FindClaim(principal, "picture") ?? string.Empty;

        return (id, email, name, avatarUrl);
    }

    private static string? FindClaim(ClaimsPrincipal principal, params string[] claimTypes)
        => claimTypes
            .Select(claimType => principal.FindFirst(claimType)?.Value)
            .FirstOrDefault(value => value is not null);
}
