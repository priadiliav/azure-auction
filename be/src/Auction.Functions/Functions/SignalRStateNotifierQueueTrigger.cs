using Auction.Application.Items;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Auction.Functions.Functions;

public class SignalRStateNotifierQueueTrigger(
    ILogger<SignalRStateNotifierQueueTrigger> logger)
{
    [Function(nameof(SignalRStateNotifierQueueTrigger))]
    [SignalROutput(HubName = "items")]
    public SignalRMessageAction Run(
        [ServiceBusTrigger(
            queueName: "states", 
            Connection = "ServiceBusConnection")] 
        ItemStateChangedMessage message)
    {
        logger.LogInformation("Notifying clients: item {ItemId} -> {Status}", message.ItemId, message.Status);
        return new SignalRMessageAction("itemStatusChanged")
        {
            Arguments = [message],
        };
    }
}
