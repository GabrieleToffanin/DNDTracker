using DNDTracker.Domain.Heroes.DomainEvents;
using NetPub.Contracts.Messaging;
using NetPub.RabbitMq.Configuration;

namespace DNDTracker.Outbound.RabbitMq.Messages;

[Provider(MessagingProvider.RabbitMq)]
[Transport(Transport.Topic)]
public record SpellCastMessage(
    string Id,
    string Source,
    SpellCastDomainEvent Payload)
    : Message<SpellCastDomainEvent>(Id, Source, Payload);

[TopicConfiguration<SpellCastMessage>]
public sealed partial class SpellCastTopicConfiguration
{
    public const string ExchangeName = "dnd.events.spell-cast";

    public RabbitMqTopicOptions Configure(RabbitMqTopicOptions options)
    {
        options.Name = ExchangeName;

        return options;
    }
}
