using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Text;

namespace DNDTracker.Outbound.RabbitMq.Messaging;

/// <summary>
/// Shared instrumentation helpers for RabbitMQ producers and consumers.
/// Uses the built-in W3C <see cref="DistributedContextPropagator"/> to inject/extract
/// trace context through AMQP message headers.
/// </summary>
public static class RabbitMqTelemetry
{
    public const string ActivitySourceName = "DNDTracker.RabbitMq";
    public static readonly Meter Meter = new(ActivitySourceName);

    private static readonly Counter<long> PublishedMessages = Meter.CreateCounter<long>(
        "dndtracker.rabbitmq.messages_published_total",
        unit: "{message}");
    private static readonly Counter<long> ConsumedMessages = Meter.CreateCounter<long>(
        "dndtracker.rabbitmq.messages_consumed_total",
        unit: "{message}");
    private static readonly Histogram<double> PublishDurationSeconds = Meter.CreateHistogram<double>(
        "dndtracker.rabbitmq.publish_duration_seconds",
        unit: "s");
    private static readonly Histogram<double> ConsumeDurationSeconds = Meter.CreateHistogram<double>(
        "dndtracker.rabbitmq.consume_duration_seconds",
        unit: "s");

    public static readonly ActivitySource ActivitySource = new(ActivitySourceName);

    public static void RecordPublish(string messageType, TimeSpan elapsed)
    {
        PublishedMessages.Add(1, new KeyValuePair<string, object?>("messaging.message.type", messageType));
        PublishDurationSeconds.Record(elapsed.TotalSeconds, new KeyValuePair<string, object?>("messaging.message.type", messageType));
    }

    public static void RecordConsume(string queueName, string messageType, TimeSpan elapsed)
    {
        ConsumedMessages.Add(1,
            new KeyValuePair<string, object?>("messaging.destination", queueName),
            new KeyValuePair<string, object?>("messaging.message.type", messageType));
        ConsumeDurationSeconds.Record(elapsed.TotalSeconds,
            new KeyValuePair<string, object?>("messaging.destination", queueName),
            new KeyValuePair<string, object?>("messaging.message.type", messageType));
    }

    public static void InjectTraceContext(Activity? activity, IDictionary<string, object?> headers)
    {
        if (activity is null)
        {
            return;
        }

        DistributedContextPropagator.Current.Inject(activity, headers, static (carrier, key, value) =>
        {
            if (carrier is IDictionary<string, object?> dictionary)
            {
                dictionary[key] = value;
            }
        });
    }

    public static (string? TraceParent, string? TraceState) ExtractTraceContext(IDictionary<string, object?>? headers)
    {
        DistributedContextPropagator.Current.ExtractTraceIdAndState(headers, static (object? carrier, string fieldName, out string? fieldValue, out IEnumerable<string>? fieldValues) =>
        {
            fieldValues = null;
            fieldValue = null;

            if (carrier is IDictionary<string, object?> dictionary && dictionary.TryGetValue(fieldName, out object? raw))
            {
                fieldValue = raw switch
                {
                    byte[] bytes => Encoding.UTF8.GetString(bytes),
                    string str => str,
                    _ => null
                };
            }
        }, out string? traceParent, out string? traceState);

        return (traceParent, traceState);
    }
}
