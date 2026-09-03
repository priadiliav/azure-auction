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
}
