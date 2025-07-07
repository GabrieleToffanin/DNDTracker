namespace DNDTracker.Domain;

public interface IEventPublisher
{
    ValueTask PublishAsync<T>(T message, CancellationToken cancellationToken = default)
        where T : notnull;
}