using DNDTracker.Domain.Campaigns.DomainEvents;
using NetPub.Contracts.Messaging;
using NetPub.RabbitMq.Configuration;

namespace DNDTracker.Outbound.RabbitMq.Messages;

[Provider(MessagingProvider.RabbitMq)]
[Transport(Transport.Topic)]
public record HeroAddedMessage(
    string Id,
    string Source,
    HeroAddedDomainEvent Payload)
    : Message<HeroAddedDomainEvent>(Id, Source, Payload);

[TopicConfiguration<HeroAddedMessage>]
public sealed partial class HeroAddedTopicConfiguration
{
    public const string ExchangeName = "dnd.events.hero-added";

    public RabbitMqTopicOptions Configure(RabbitMqTopicOptions options)
    {
        options.Name = ExchangeName;

        return options;
    }
}
