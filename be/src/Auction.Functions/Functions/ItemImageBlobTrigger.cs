using Auction.Application.Items;
using Auction.Contracts;
using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Auction.Functions.Functions;

public class ItemImageBlobTrigger(
    IItemRepository itemRepository,
    IItemPublisher itemPublisher,
    ILogger<ItemImageBlobTrigger> logger)
{
    [Function(nameof(ItemImageBlobTrigger))]
    public async Task Run(
        [BlobTrigger("items/{itemId}/{fileName}", Connection = "BlobStorage")] BlobClient blobClient,
        Guid itemId,
        string fileName,
        CancellationToken cancellationToken)
    {
        var item = await itemRepository.GetByIdAsync(itemId, cancellationToken);
        if (item is null)
        {
            logger.LogWarning("Received blob {FileName} for unknown item {ItemId}, ignoring", fileName, itemId);
            return;
        }

        item.SetBlobUrl(blobClient.Uri.ToString());
        await itemRepository.UpdateAsync(item, cancellationToken);

        await itemPublisher.PublishAsync(new ItemCreatedMessage(itemId), cancellationToken);

        logger.LogInformation("Item {ItemId} image uploaded, processing pipeline started", itemId);
    }
}
