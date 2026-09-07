using System.Text.Json;

namespace Auction.Application.Items;

public static class EmbeddingVector
{
    public static string Serialize(float[] embedding) => JsonSerializer.Serialize(embedding);

    public static float[] Parse(string embeddingJson) => JsonSerializer.Deserialize<float[]>(embeddingJson) ?? [];

    public static double Cosine(float[] a, float[] b)
    {
        if (a.Length == 0 || b.Length == 0 || a.Length != b.Length)
        {
            return 0;
        }

        double dot = 0, normA = 0, normB = 0;
        for (var i = 0; i < a.Length; i++)
        {
            dot += a[i] * b[i];
            normA += a[i] * a[i];
            normB += b[i] * b[i];
        }

        var denominator = Math.Sqrt(normA) * Math.Sqrt(normB);
        return denominator == 0 ? 0 : dot / denominator;
    }
}
