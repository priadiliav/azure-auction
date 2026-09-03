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

    /// <summary>
    /// Retrieves all items.
    /// </summary>
    Task<IReadOnlyList<Item>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Persists changes made to an existing item.
    /// </summary>
    Task UpdateAsync(Item item, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves items whose auction end time has passed but that haven't been closed yet.
    /// </summary>
    Task<IReadOnlyList<Item>> GetExpiredActiveAsync(DateTimeOffset now, CancellationToken cancellationToken);
}
