using DNDTracker.Domain;
using DNDTracker.SharedKernel.Primitives;
using FluentAssertions;

namespace DNDTracker.Application.Tests.Behaviors.Dummies;

public class DummyEventPublisher : IEventPublisher
{
    public List<object> PublishedEvents { get; } = new();
    
    public async ValueTask PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : notnull
    {
        PublishedEvents.Add(message);
        await Task.CompletedTask;
    }
    
    internal void AssertEventPublished<T>() where T : DomainEvent
    {
        PublishedEvents.Should().ContainSingle(e => e is T);
    }
    
    internal void AssertEventPublished<T>(T expectedEvent) where T : DomainEvent
    {
        PublishedEvents.Should().Contain(expectedEvent);
    }
    
    internal void AssertEventsPublished(int count)
    {
        PublishedEvents.Should().HaveCount(count);
    }
    
    internal void AssertNoEventsPublished()
    {
        PublishedEvents.Should().BeEmpty();
    }
    
    internal T GetPublishedEvent<T>() where T : DomainEvent
    {
        return PublishedEvents.OfType<T>().Single();
    }
    
    internal void Clear()
    {
        PublishedEvents.Clear();
    }
}