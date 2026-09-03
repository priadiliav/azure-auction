using Auction.Application.Items;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Auction.Functions.Functions;

public class SignalRBidNotifierQueueTrigger(
    ILogger<SignalRBidNotifierQueueTrigger> logger)
{
    [Function(nameof(SignalRBidNotifierQueueTrigger))]
    [SignalROutput(HubName = "items")]
    public SignalRMessageAction Run(
        [ServiceBusTrigger(
            queueName: "bids",
            Connection = "ServiceBusConnection")]
        BidPlacedMessage message)
    {
        logger.LogInformation(
            "Notifying clients: item {ItemId} new bid {Amount} by {BidderName}",
            message.ItemId, message.Amount, message.BidderName);
        return new SignalRMessageAction("bidPlaced")
        {
            Arguments = [message],
        };
    }
}
