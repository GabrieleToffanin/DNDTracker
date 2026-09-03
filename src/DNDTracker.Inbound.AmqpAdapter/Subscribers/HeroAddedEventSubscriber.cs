using DNDTracker.Outbound.RabbitMq.Messages;
using Microsoft.Extensions.Logging;
using NetPub.Contracts.Messaging;
using NetPub.RabbitMq.Configuration;

namespace DNDTracker.Inbound.AmqpAdapter.Subscribers;

[HandlesMessage<HeroAddedMessage>]
[Transport(Transport.Topic)]
public sealed partial class HeroAddedEventSubscriber(ILogger<HeroAddedEventSubscriber> logger)
{
    public ValueTask HandleAsync(HeroAddedMessage message, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "🦸 Received hero-added event: Id={EventId}, OccuredOn={OccuredOn}, Source={Source}",
            message.Payload.Id,
            message.Payload.OccuredOn,
            message.Source);

        return ValueTask.CompletedTask;
    }
}

[SubscriberConfiguration<HeroAddedEventSubscriber>]
public sealed partial class HeroAddedSubscriptionConfiguration
{
    public const string QueueName = "dndtracking.campaign.hero-added";

    public RabbitMqSubscriptionOptions Configure(RabbitMqSubscriptionOptions options)
    {
        options.Name = QueueName;

        return options;
    }
}
