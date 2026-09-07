using Auction.Application.Items;
using Auction.Domain.Interactions;
using Microsoft.EntityFrameworkCore;

namespace Auction.Infrastructure.Persistence;

public class InteractionRepository(AuctionDbContext dbContext) : IInteractionRepository
{
    private static readonly Dictionary<InteractionType, double> Weights = new()
    {
        [InteractionType.Viewed] = 1,
        [InteractionType.Bid] = 2,
        [InteractionType.Win] = 3,
    };

    public async Task AddAsync(Interaction interaction, CancellationToken cancellationToken)
    {
        dbContext.Interactions.Add(interaction);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<float[]?> GetUserTasteVectorAsync(string userId, CancellationToken cancellationToken)
    {
        var interactions = await dbContext.Interactions
            .Where(interaction => interaction.UserId == userId)
            .Join(
                dbContext.Items,
                interaction => interaction.ItemId,
                item => item.Id,
                (interaction, item) => new { interaction.Type, item.Embedding })
            .Where(x => x.Embedding != null)
            .ToListAsync(cancellationToken);

        if (interactions.Count == 0)
        {
            return null;
        }

        float[]? weightedSum = null;
        double totalWeight = 0;

        foreach (var interaction in interactions)
        {
            var vector = EmbeddingVector.Parse(interaction.Embedding!);
            var weight = Weights[interaction.Type];

            weightedSum ??= new float[vector.Length];
            for (var i = 0; i < vector.Length && i < weightedSum.Length; i++)
            {
                weightedSum[i] += (float)(vector[i] * weight);
            }

            totalWeight += weight;
        }

        if (weightedSum is null || totalWeight == 0)
        {
            return null;
        }

        for (var i = 0; i < weightedSum.Length; i++)
        {
            weightedSum[i] = (float)(weightedSum[i] / totalWeight);
        }

        return weightedSum;
    }
}
