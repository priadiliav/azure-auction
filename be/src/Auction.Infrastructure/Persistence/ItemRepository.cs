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
}
