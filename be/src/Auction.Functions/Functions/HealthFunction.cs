using System.Reflection;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Auction.Functions.Functions;

public class HealthFunction(ILogger<HealthFunction> logger)
{
    [Function("HealthFunction")]
    public IActionResult Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous, 
            "get", 
            "post")] 
        HttpRequest req)
    {
        logger.LogInformation("Health check function processed a request.");
        var version = Assembly.GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
            ?? "unknown";
        var response = new
        {
            status = "healthy",
            version,
            environment = Environment.GetEnvironmentVariable("AZURE_FUNCTIONS_ENVIRONMENT") ?? "unknown",
        };

        return new OkObjectResult(response);
    }
}