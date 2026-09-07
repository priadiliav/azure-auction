namespace Auction.Application.Items;

public record InteractionRecordedMessage(string UserId, Guid ItemId, string Type);

public interface IInteractionPublisher
{
    Task PublishAsync(InteractionRecordedMessage message, CancellationToken cancellationToken);
}
