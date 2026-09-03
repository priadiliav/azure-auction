using Auction.Application.Users;
using Auction.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Auction.Infrastructure.Persistence;

public class UserRepository(AuctionDbContext dbContext) : IUserRepository
{
    public Task<User?> GetByIdAsync(string id, CancellationToken cancellationToken)
        => dbContext.Users.SingleOrDefaultAsync(u => u.Id == id, cancellationToken);

    public async Task UpsertAsync(User user, CancellationToken cancellationToken)
    {
        var existing = await dbContext.Users.SingleOrDefaultAsync(u => u.Id == user.Id, cancellationToken);
        if (existing is null)
        {
            dbContext.Users.Add(user);
        }
        else
        {
            existing.UpdateProfile(user.Email, user.Name, user.AvatarUrl);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
