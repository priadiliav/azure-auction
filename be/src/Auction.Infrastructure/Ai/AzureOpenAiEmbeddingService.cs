using Auction.Application.Items;
using OpenAI.Embeddings;

namespace Auction.Infrastructure.Ai;

public class AzureOpenAiEmbeddingService(EmbeddingClient embeddingClient) : IEmbeddingService
{
    public async Task<float[]> GetEmbeddingAsync(string text, CancellationToken cancellationToken)
    {
        var result = await embeddingClient.GenerateEmbeddingAsync(text, cancellationToken: cancellationToken);
        return result.Value.ToFloats().ToArray();
    }
}
