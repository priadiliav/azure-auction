using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Auction.Functions.Functions;

public class SignalRNegotiateHttpTrigger(
    ILogger<SignalRNegotiateHttpTrigger> logger)
{
    [Function(nameof(SignalRNegotiateHttpTrigger))]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "negotiate")]
        HttpRequestData req,
        [SignalRConnectionInfoInput(
            HubName = "items",
            ConnectionStringSetting = "AzureSignalRConnectionString")]
        string connectionInfo)
    {
        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json; charset=utf-8");
        await response.WriteStringAsync(connectionInfo);
        logger.LogInformation("SignalR negotiation request processed successfully.");
        return response;
    }
}
