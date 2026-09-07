using Auction.Domain.Interactions;

namespace Auction.Application.Items;

public interface IInteractionRepository
{
    Task AddAsync(Interaction interaction, CancellationToken cancellationToken);

    /// <summary>
    /// Computes the user's "taste vector" as a weighted average of the embeddings of items
    /// they've interacted with (Win > Bid > Viewed). Returns null if they have no interactions
    /// with items that have an embedding yet.
    /// </summary>
    Task<float[]?> GetUserTasteVectorAsync(string userId, CancellationToken cancellationToken);
}
