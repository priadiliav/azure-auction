using System.Text.Json;
using Auction.Application.Items;
using Azure.Messaging.ServiceBus;

namespace Auction.Infrastructure.Messaging;

public class ServiceBusBidPublisher(ServiceBusClient client) : IBidPublisher
{
    private readonly ServiceBusSender _sender = client.CreateSender("bids");

    public async Task PublishAsync(BidPlacedMessage message, CancellationToken cancellationToken)
    {
        var body = JsonSerializer.Serialize(message);
        await _sender.SendMessageAsync(new ServiceBusMessage(body), cancellationToken);
    }
}
