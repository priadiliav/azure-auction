namespace Auction.Application.Items;

public record BlobUploadSas(string UploadUrl, string BlobUrl, DateTimeOffset ExpiresOn);
