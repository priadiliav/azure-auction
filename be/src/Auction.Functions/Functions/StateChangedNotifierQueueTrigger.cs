using Auction.Contracts;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.SignalRService;
using Microsoft.Extensions.Logging;

namespace Auction.Functions.Functions;

public class StateChangedNotifierQueueTrigger(ILogger<StateChangedNotifierQueueTrigger> logger)
{
    [Function(nameof(StateChangedNotifierQueueTrigger))]
    [SignalROutput(
        HubName = "items", 
        ConnectionStringSetting = "AzureSignalRConnection")]
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
