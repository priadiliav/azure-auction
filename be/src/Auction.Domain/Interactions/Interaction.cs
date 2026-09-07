namespace Auction.Domain.Interactions;

public class Interaction
{
    public Guid Id { get; private set; }
    public string UserId { get; private set; } = string.Empty;
    public Guid ItemId { get; private set; }
    public InteractionType Type { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private Interaction()
    {
    }

    public Interaction(Guid id, string userId, Guid itemId, InteractionType type, DateTimeOffset createdAt)
    {
        Id = id;
        UserId = userId;
        ItemId = itemId;
        Type = type;
        CreatedAt = createdAt;
    }
}
