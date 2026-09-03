using Auction.Application.Items;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;

namespace Auction.Infrastructure.Storage;

public class BlobSasService(BlobServiceClient blobServiceClient) : IBlobSasService
{
    private const string ContainerName = "items";
    private static readonly TimeSpan SasLifetime = TimeSpan.FromMinutes(15);

    public async Task<BlobUploadSas> CreateUploadSasAsync(
        Guid itemId,
        string fileName,
        string contentType,
        CancellationToken cancellationToken)
    {
        var extension = Path.GetExtension(fileName);
        var blobName = $"{itemId}/{Guid.NewGuid()}{extension}";
        var blobClient = blobServiceClient.GetBlobContainerClient(ContainerName).GetBlobClient(blobName);

        var startsOn = DateTimeOffset.UtcNow.AddMinutes(-5);
        var expiresOn = DateTimeOffset.UtcNow.Add(SasLifetime);

        var userDelegationKey = await blobServiceClient.GetUserDelegationKeyAsync(startsOn, expiresOn, cancellationToken);

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = ContainerName,
            BlobName = blobName,
            Resource = "b",
            StartsOn = startsOn,
            ExpiresOn = expiresOn,
        };
        sasBuilder.SetPermissions(BlobSasPermissions.Create | BlobSasPermissions.Write);

        var sasQuery = sasBuilder.ToSasQueryParameters(userDelegationKey, blobServiceClient.AccountName);
        var uploadUrl = new UriBuilder(blobClient.Uri) { Query = sasQuery.ToString() }.Uri.ToString();

        return new BlobUploadSas(uploadUrl, blobClient.Uri.ToString(), expiresOn);
    }
}
