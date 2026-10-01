namespace AtlasOps.Connectors.Observability;

using System.Collections.Concurrent;
using System.Diagnostics;

using AtlasOps.Connectors.Contracts;

using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Expressions;
using Serilog.Sinks.File;

public sealed record ObservabilityPackageDescriptor(string Id, Type PrimaryType, string Capability);

public sealed record ConnectorMetricSnapshot(
    string ConnectorId,
    long Succeeded,
    long Failed,
    long Throttled,
    long Cancelled,
    TimeSpan TotalDuration,
    DateTimeOffset CapturedAt);

public static class ObservabilityPackageCatalog
{
    public static IReadOnlyList<ObservabilityPackageDescriptor> Packages { get; } =
    [
        new("serilog-aspnetcore", typeof(LoggerConfiguration), "Host logging integration"),
        new("environment-enricher", typeof(Serilog.Configuration.LoggerEnrichmentConfiguration), "Environment metadata"),
        new("expressions", typeof(SerilogExpression), "Expression-based filtering"),
        new("async-sink", typeof(Serilog.Configuration.LoggerSinkConfiguration), "Asynchronous log delivery"),
        new("debug-sink", typeof(Serilog.Debugging.SelfLog), "Debug output"),
        new("file-sink", typeof(FileLifecycleHooks), "Durable local logs"),
        new("diagnostic-source", typeof(ActivitySource), "Distributed trace correlation"),
    ];
}

public sealed class ConnectorMetrics
{
    private readonly ConcurrentDictionary<string, MutableMetric> metrics =
        new(StringComparer.OrdinalIgnoreCase);

    public void Record(ConnectorAuditRecord record)
    {
        MutableMetric metric = this.metrics.GetOrAdd(record.ConnectorId, static _ => new MutableMetric());
        metric.Record(record.Status, record.Duration);
    }

    public IReadOnlyList<ConnectorMetricSnapshot> Snapshot()
    {
        DateTimeOffset capturedAt = DateTimeOffset.UtcNow;
        return this.metrics
            .Select(item => item.Value.Snapshot(item.Key, capturedAt))
            .OrderBy(static item => item.ConnectorId, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private sealed class MutableMetric
    {
        private long succeeded;
        private long failed;
        private long throttled;
        private long cancelled;
        private long durationTicks;

        public void Record(ConnectorExecutionStatus status, TimeSpan duration)
        {
            switch (status)
            {
                case ConnectorExecutionStatus.Succeeded:
                    Interlocked.Increment(ref this.succeeded);
                    break;
                case ConnectorExecutionStatus.Throttled:
                    Interlocked.Increment(ref this.throttled);
                    break;
                case ConnectorExecutionStatus.Cancelled:
                    Interlocked.Increment(ref this.cancelled);
                    break;
                default:
                    Interlocked.Increment(ref this.failed);
                    break;
            }

            Interlocked.Add(ref this.durationTicks, duration.Ticks);
        }

        public ConnectorMetricSnapshot Snapshot(string connectorId, DateTimeOffset capturedAt)
        {
            return new ConnectorMetricSnapshot(
                connectorId,
                Interlocked.Read(ref this.succeeded),
                Interlocked.Read(ref this.failed),
                Interlocked.Read(ref this.throttled),
                Interlocked.Read(ref this.cancelled),
                TimeSpan.FromTicks(Interlocked.Read(ref this.durationTicks)),
                capturedAt);
        }
    }
}

public sealed class ConnectorActivityScope : IDisposable
{
    private static readonly ActivitySource Source = new("AtlasOps.Connectors");
    private readonly Activity? activity;

    public ConnectorActivityScope(ConnectorExecutionRequest request)
    {
        this.activity = Source.StartActivity("connector.execute", ActivityKind.Internal);
        this.activity?.SetTag("connector.id", request.ConnectorId);
        this.activity?.SetTag("connector.operation", request.Operation);
        this.activity?.SetTag("connector.resource", request.ResourceId);
        this.activity?.SetTag("connector.execution_id", request.ExecutionId);
    }

    public string? TraceId => this.activity?.TraceId.ToString();

    public void Complete(ConnectorExecutionOutcome outcome)
    {
        this.activity?.SetTag("connector.status", outcome.Status.ToString());
        this.activity?.SetTag("connector.attempts", outcome.Attempts);
        this.activity?.SetStatus(
            outcome.Status == ConnectorExecutionStatus.Succeeded
                ? ActivityStatusCode.Ok
                : ActivityStatusCode.Error,
            outcome.Code);
    }

    public void Dispose()
    {
        this.activity?.Dispose();
    }
}
