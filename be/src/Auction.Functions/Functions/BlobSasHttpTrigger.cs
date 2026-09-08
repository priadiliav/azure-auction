using System.Net;
using System.Text.Json;
using Auction.Application.Items;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Auction.Functions.Functions;

public class BlobSasHttpTrigger(
    IItemRepository itemRepository,
    IBlobSasService blobSasService,
    ILogger<BlobSasHttpTrigger> logger)
{
    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png",
        "image/webp",
    };
    

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    [Function(nameof(BlobSasHttpTrigger))]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, 
            "post", 
            Route = "items/{itemId:guid}/blob-sas")]
        HttpRequestData req,
        Guid itemId,
        CancellationToken cancellationToken)
    {
        var query = QueryHelpers.ParseQuery(req.Url.Query);
        
        var fileName = query.TryGetValue("fileName", out var fileNameValue) ? fileNameValue.ToString() : null;
        var contentType = query.TryGetValue("contentType", out var contentTypeValue) ? contentTypeValue.ToString() : null;

        if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(contentType))
        {
            return await CreateTextResponseAsync(req, HttpStatusCode.BadRequest, "fileName and contentType query parameters are required.");
        }

        if (!AllowedContentTypes.Contains(contentType))
        {
            return await CreateTextResponseAsync(req, HttpStatusCode.UnsupportedMediaType, $"Content type '{contentType}' is not allowed.");
        }

        var item = await itemRepository.GetByIdAsync(itemId, cancellationToken);
        if (item is null)
        {
            return await CreateTextResponseAsync(req, HttpStatusCode.NotFound, $"Item '{itemId}' was not found.");
        }

        var sas = await blobSasService.CreateUploadSasAsync(itemId, fileName, contentType, cancellationToken);

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json; charset=utf-8");
        await response.WriteStringAsync(JsonSerializer.Serialize(sas, JsonOptions), cancellationToken);

        logger.LogInformation("Issued upload SAS for item {ItemId}", itemId);
        return response;
    }

    private static async Task<HttpResponseData> CreateTextResponseAsync(HttpRequestData req, HttpStatusCode statusCode, string message)
    {
        var response = req.CreateResponse(statusCode);
        await response.WriteStringAsync(message);
        return response;
    }
}
