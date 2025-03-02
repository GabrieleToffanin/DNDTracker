using DNDTracker.Domain;

namespace DNDTracker.Outbound.InMemoryAdapter.Messaging;

public class EventPublisher : IEventPublisher
{
    public async ValueTask<T> PublishAsync<T>(T message, CancellationToken cancellationToken = default) 
        where T : notnull
    {
        // Nothing
        return await Task.FromResult(message);
    }
}