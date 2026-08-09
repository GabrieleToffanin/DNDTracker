using System.Diagnostics;
using System.Text;
using Datadog.Trace;

namespace DNDTracker.Outbound.RabbitMq.Messaging;

/// <summary>
/// Shared instrumentation helpers for RabbitMQ producers and consumers.
/// Uses the built-in W3C <see cref="DistributedContextPropagator"/> to inject/extract
/// trace context through AMQP message headers, since there is no official
/// RabbitMQ.Client instrumentation package for .NET. Activities are also tagged with
/// the native Datadog trace/span ids so they can be correlated with APM traces and logs.
/// </summary>
public static class RabbitMqTelemetry
{
    public const string ActivitySourceName = "DNDTracker.RabbitMq";

    public static readonly ActivitySource ActivitySource = new(ActivitySourceName);

    /// <summary>
    /// Tags the given activity with the current Datadog trace/span identifiers, if a
    /// Datadog APM span is active, so it can be correlated with the request trace and logs.
    /// </summary>
    public static void TagDatadogCorrelation(Activity? activity)
    {
        if (activity is null)
        {
            return;
        }

        ISpan? activeSpan = Tracer.Instance.ActiveScope?.Span;

        activity.SetTag("dd.trace_id", activeSpan?.TraceId.ToString() ?? "0");
        activity.SetTag("dd.span_id", activeSpan?.SpanId.ToString() ?? "0");
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
