using DNDTracker.Domain.Heroes.DomainEvents;
using NetPub.Contracts.Messaging;
using NetPub.RabbitMq.Configuration;

namespace DNDTracker.Outbound.RabbitMq.Messages;

[Provider(MessagingProvider.RabbitMq)]
[Transport(Transport.Topic)]
public record SpellLearnedMessage(
    string Id,
    string Source,
    SpellLearnedDomainEvent Payload)
    : Message<SpellLearnedDomainEvent>(Id, Source, Payload);

[TopicConfiguration<SpellLearnedMessage>]
public sealed partial class SpellLearnedTopicConfiguration
{
    public const string ExchangeName = "dnd.events.spell-learned";

    public RabbitMqTopicOptions Configure(RabbitMqTopicOptions options)
    {
        options.Name = ExchangeName;

        return options;
    }
}
