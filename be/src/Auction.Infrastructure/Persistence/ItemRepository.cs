using Auction.Application.Items;
using Auction.Domain.Items;
using Microsoft.EntityFrameworkCore;

namespace Auction.Infrastructure.Persistence;

public class ItemRepository(AuctionDbContext dbContext) : IItemRepository
{
    public async Task AddAsync(Item item, CancellationToken cancellationToken)
    {
        dbContext.Items.Add(item);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<Item?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => dbContext.Items.SingleOrDefaultAsync(item => item.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Item>> GetAllAsync(CancellationToken cancellationToken)
        => await dbContext.Items.OrderByDescending(item => item.Id).ToListAsync(cancellationToken);

    public async Task UpdateAsync(Item item, CancellationToken cancellationToken)
    {
        dbContext.Items.Update(item);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Item>> GetExpiredActiveAsync(DateTimeOffset now, CancellationToken cancellationToken)
        => await dbContext.Items
            .Where(item => item.Status != ItemStatus.Ended && item.EndsAt <= now)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Item>> GetBySellerIdAsync(string sellerId, CancellationToken cancellationToken)
        => await dbContext.Items
            .Where(item => item.SellerId == sellerId)
            .OrderByDescending(item => item.Id)
            .ToListAsync(cancellationToken);
}
