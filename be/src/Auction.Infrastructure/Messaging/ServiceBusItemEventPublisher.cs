using System.Text.Json;
using Auction.Application.Items.CreateItem;
using Auction.Contracts;
using Azure.Messaging.ServiceBus;

namespace Auction.Infrastructure.Messaging;

public class ServiceBusItemEventPublisher(ServiceBusClient client) : IItemEventPublisher
{
    private readonly ServiceBusSender _sender = client.CreateSender("items");

    public async Task PublishItemCreatedAsync(ItemCreatedMessage message, CancellationToken cancellationToken)
    {
        var body = JsonSerializer.Serialize(message);
        await _sender.SendMessageAsync(new ServiceBusMessage(body), cancellationToken);
    }
}
