using Auction.Application.Items;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Auction.Functions.Functions;

public class ItemCloserTimerTrigger(
    IItemRepository itemRepository,
    IStatePublisher statePublisher,
    ILogger<ItemCloserTimerTrigger> logger)
{
    [Function(nameof(ItemCloserTimerTrigger))]
    public async Task Run(
        [TimerTrigger("0 * * * * *")] TimerInfo timer,
        CancellationToken cancellationToken)
    {
        var expiredItems = await itemRepository.GetExpiredActiveAsync(DateTimeOffset.UtcNow, cancellationToken);

        foreach (var item in expiredItems)
        {
            item.Close();
            await itemRepository.UpdateAsync(item, cancellationToken);
            await statePublisher.PublishAsync(new ItemStateChangedMessage(item.Id, item.Status.ToString()), cancellationToken);

            logger.LogInformation(
                "Item {ItemId} auction closed, final price {CurrentPrice}", item.Id, item.CurrentPrice);
        }
    }
}
