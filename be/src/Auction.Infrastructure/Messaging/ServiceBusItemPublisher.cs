using System.Text.Json;
using Auction.Application.Items;
using Auction.Contracts;
using Azure.Messaging.ServiceBus;

namespace Auction.Infrastructure.Messaging;

public class ServiceBusItemPublisher(ServiceBusClient client) : IItemPublisher
{
    private readonly ServiceBusSender _sender = client.CreateSender("items");

    public async Task PublishAsync(ItemCreatedMessage message, CancellationToken cancellationToken)
    {
        var body = JsonSerializer.Serialize(message);
        await _sender.SendMessageAsync(new ServiceBusMessage(body), cancellationToken);
    }
}
