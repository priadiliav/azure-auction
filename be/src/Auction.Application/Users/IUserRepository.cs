using Auction.Domain.Users;

namespace Auction.Application.Users;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(string id, CancellationToken cancellationToken);

    /// <summary>
    /// Creates the user if they don't exist yet, or refreshes their profile (name/email/avatar
    /// can change on Google's side) if they do.
    /// </summary>
    Task UpsertAsync(User user, CancellationToken cancellationToken);
}
