using System.Text.Json;
using Auction.Application.Items;
using Azure.Messaging.ServiceBus;

namespace Auction.Infrastructure.Messaging;

public class ServiceBusInteractionPublisher(ServiceBusClient client) : IInteractionPublisher
{
    private readonly ServiceBusSender _sender = client.CreateSender("interactions");

    public async Task PublishAsync(InteractionRecordedMessage message, CancellationToken cancellationToken)
    {
        var body = JsonSerializer.Serialize(message);
        await _sender.SendMessageAsync(new ServiceBusMessage(body), cancellationToken);
    }
}
