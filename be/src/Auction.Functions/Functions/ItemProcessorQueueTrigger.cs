using Auction.Application.Items;
using Auction.Contracts;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Auction.Functions.Functions;

public class ItemProcessorQueueTrigger(
    IItemRepository itemRepository,
    IStatePublisher statePublisher,
    ILogger<ItemProcessorQueueTrigger> logger)
{
    private static readonly TimeSpan StepDelay = TimeSpan.FromSeconds(5);

    [Function(nameof(ItemProcessorQueueTrigger))]
    public async Task Run(
        [ServiceBusTrigger("items", Connection = "ServiceBusConnection")] ItemCreatedMessage message,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Processing item {ItemId}", message.ItemId);

        var item = await itemRepository.GetByIdAsync(message.ItemId, cancellationToken);
        if (item is null)
        {
            logger.LogWarning("Item {ItemId} not found", message.ItemId);
            return;
        }

        item.MarkInProcess();
        await itemRepository.UpdateAsync(item, cancellationToken);
        await statePublisher.PublishAsync(new ItemStateChangedMessage(item.Id, item.Status.ToString()), cancellationToken);
        await Task.Delay(StepDelay, cancellationToken);

        item.MarkAnalysis();
        await itemRepository.UpdateAsync(item, cancellationToken);
        await statePublisher.PublishAsync(new ItemStateChangedMessage(item.Id, item.Status.ToString()), cancellationToken);
        await Task.Delay(StepDelay, cancellationToken);

        item.MarkListed();
        await itemRepository.UpdateAsync(item, cancellationToken);
        await statePublisher.PublishAsync(new ItemStateChangedMessage(item.Id, item.Status.ToString()), cancellationToken);

        logger.LogInformation("Item {ItemId} listed", item.Id);
    }
}
