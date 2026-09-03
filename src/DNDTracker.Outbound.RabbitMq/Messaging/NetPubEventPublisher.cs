using DNDTracker.Domain;
using DNDTracker.Domain.Campaigns.DomainEvents;
using DNDTracker.Domain.Heroes.DomainEvents;
using DNDTracker.Outbound.RabbitMq.Messages;
using NetPub.Publishing;

namespace DNDTracker.Outbound.RabbitMq.Messaging;

/// <summary>
/// Adapts the domain <see cref="IEventPublisher"/> port to NetPub by wrapping each domain event
/// in its RabbitMQ message contract. Destination resolution, serialization, publisher confirms
/// and telemetry are handled by NetPub.
/// </summary>
internal sealed class NetPubEventPublisher(IPublisher publisher) : IEventPublisher
{
    public const string Source = "dndtracker";

    public ValueTask PublishAsync<T>(T message, CancellationToken cancellationToken = default)
        where T : notnull
    {
        cancellationToken.ThrowIfCancellationRequested();

        return message switch
        {
            HeroAddedDomainEvent domainEvent => publisher.PublishAsync(
                new HeroAddedMessage(domainEvent.Id.ToString(), Source, domainEvent),
                cancellationToken),
            SpellLearnedDomainEvent domainEvent => publisher.PublishAsync(
                new SpellLearnedMessage(domainEvent.Id.ToString(), Source, domainEvent),
                cancellationToken),
            SpellCastDomainEvent domainEvent => publisher.PublishAsync(
                new SpellCastMessage(domainEvent.Id.ToString(), Source, domainEvent),
                cancellationToken),
            _ => throw new InvalidOperationException(
                $"No RabbitMQ message contract is defined for event type '{message.GetType().FullName}'.")
        };
    }
}
