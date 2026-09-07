using Auction.Application.Items;
using Auction.Domain.Interactions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Auction.Functions.Functions;

public class InteractionRecorderQueueTrigger(
    IInteractionRepository interactionRepository,
    ILogger<InteractionRecorderQueueTrigger> logger)
{
    [Function(nameof(InteractionRecorderQueueTrigger))]
    public async Task Run(
        [ServiceBusTrigger("interactions", Connection = "ServiceBusConnection")] InteractionRecordedMessage message,
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<InteractionType>(message.Type, out var type))
        {
            logger.LogWarning("Unknown interaction type {Type}, ignoring", message.Type);
            return;
        }

        var interaction = new Interaction(Guid.CreateVersion7(), message.UserId, message.ItemId, type, DateTimeOffset.UtcNow);
        await interactionRepository.AddAsync(interaction, cancellationToken);

        logger.LogInformation(
            "Recorded {Type} interaction for user {UserId} on item {ItemId}", type, message.UserId, message.ItemId);
    }
}
