using Auction.Application.Items;
using Auction.Domain.Interactions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Auction.Functions.Functions;

public class ItemCloserTimerTrigger(
    IItemRepository itemRepository,
    IStatePublisher statePublisher,
    IBidRepository bidRepository,
    IInteractionPublisher interactionPublisher,
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

            var winningBid = (await bidRepository.GetRecentBidsAsync(item.Id, 1, cancellationToken)).FirstOrDefault();
            if (winningBid is not null)
            {
                await interactionPublisher.PublishAsync(
                    new InteractionRecordedMessage(winningBid.BidderId, item.Id, nameof(InteractionType.Win)),
                    cancellationToken);
            }

            logger.LogInformation(
                "Item {ItemId} auction closed, final price {CurrentPrice}", item.Id, item.CurrentPrice);
        }
    }
}
