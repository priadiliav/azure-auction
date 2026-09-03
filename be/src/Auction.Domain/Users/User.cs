namespace Auction.Domain.Users;

public class User
{
    public string Id { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string AvatarUrl { get; private set; } = string.Empty;

    private User()
    {
    }

    public User(string id, string email, string name, string avatarUrl)
    {
        Id = id;
        Email = email;
        Name = name;
        AvatarUrl = avatarUrl;
    }

    public void UpdateProfile(string email, string name, string avatarUrl)
    {
        Email = email;
        Name = name;
        AvatarUrl = avatarUrl;
    }
}
