namespace DNDTracker.Domain;

public interface IEventPublisher
{
    ValueTask<T> PublishAsync<T>(T message, CancellationToken cancellationToken = default)
        where T : notnull;
}