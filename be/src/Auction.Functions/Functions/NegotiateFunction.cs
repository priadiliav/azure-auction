using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Auction.Functions.Functions;

public class NegotiateFunction
{
    [Function(nameof(NegotiateFunction))]
    public static string Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "negotiate")]
        HttpRequestData req,
        [SignalRConnectionInfoInput(HubName = "items", ConnectionStringSetting = "AzureSignalRConnection")]
        string connectionInfo) => connectionInfo;
}
