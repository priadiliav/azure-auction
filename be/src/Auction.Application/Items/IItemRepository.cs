using Auction.Domain.Items;

namespace Auction.Application.Items;

public interface IItemRepository
{
    /// <summary>
    /// Adds a new item to the repository.
    /// </summary>
    Task AddAsync(Item item, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves an item by its unique identifier.
    /// </summary>
    Task<Item?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}
