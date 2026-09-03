using System.Text.Json;
using Auction.Application.Items;
using Azure.Messaging.ServiceBus;

namespace Auction.Infrastructure.Messaging;

public class ServiceBusStatePublisher(ServiceBusClient client) : IStatePublisher
{
    private readonly ServiceBusSender _sender = client.CreateSender("states");
    
    public async Task PublishAsync(ItemStateChangedMessage message, CancellationToken cancellationToken)
    {
        var body = JsonSerializer.Serialize(message);
        await _sender.SendMessageAsync(new ServiceBusMessage(body), cancellationToken);
    }
}