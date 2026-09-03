namespace Auction.Application.Items;

public interface IBlobSasService
{
    /// <summary>
    /// Mint a short-lived, write-only SAS for a client to upload an item's image directly to blob storage.
    /// </summary>
    Task<BlobUploadSas> CreateUploadSasAsync(
        Guid itemId,
        string fileName,
        string contentType,
        CancellationToken cancellationToken);
}
