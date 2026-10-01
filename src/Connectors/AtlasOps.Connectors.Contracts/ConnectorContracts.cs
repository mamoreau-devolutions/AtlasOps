namespace AtlasOps.Connectors.Contracts;

using System.Collections.ObjectModel;

public enum ConnectorKind
{
    Http,
    Collaboration,
    Database,
    Identity,
    Messaging,
    Document,
    Observability,
    Custom,
}

public enum ConnectorHealthState
{
    Unknown,
    Healthy,
    Degraded,
    Unhealthy,
    Disabled,
}

public enum ConnectorExecutionStatus
{
    Succeeded,
    Failed,
    Throttled,
    Disabled,
    NotFound,
    Cancelled,
}

public sealed record ConnectorDefinition(
    string Id,
    string DisplayName,
    ConnectorKind Kind,
    bool Enabled,
    int MaximumConcurrency,
    int RequestsPerMinute,
    int MaximumAttempts,
    TimeSpan Timeout,
    IReadOnlyDictionary<string, string> Metadata);

public sealed record ConnectorCredentialReference(
    string ResolverId,
    string CredentialId,
    string? Version);

public sealed record ConnectorExecutionRequest(
    Guid ExecutionId,
    string ConnectorId,
    string Operation,
    string ResourceId,
    ConnectorCredentialReference? Credential,
    DateTimeOffset RequestedAt,
    IReadOnlyDictionary<string, string> Parameters,
    string? CorrelationId = null);

public sealed record ConnectorExecutionOutcome(
    Guid ExecutionId,
    ConnectorExecutionStatus Status,
    string Code,
    string Message,
    int Attempts,
    DateTimeOffset StartedAt,
    DateTimeOffset CompletedAt,
    IReadOnlyDictionary<string, string> Details)
{
    public TimeSpan Duration => this.CompletedAt - this.StartedAt;
}

public sealed record ConnectorHealthSnapshot(
    string ConnectorId,
    ConnectorHealthState State,
    string Message,
    DateTimeOffset CheckedAt,
    TimeSpan Latency,
    IReadOnlyDictionary<string, string> Details);

public sealed record ConnectorAuditRecord(
    Guid EventId,
    Guid ExecutionId,
    string ConnectorId,
    string Operation,
    string ResourceId,
    ConnectorExecutionStatus Status,
    int Attempts,
    DateTimeOffset OccurredAt,
    TimeSpan Duration,
    string? CorrelationId,
    IReadOnlyDictionary<string, string> Details);

public sealed record ConnectorCheckpoint(
    string ConnectorId,
    string Partition,
    string Cursor,
    long Revision,
    DateTimeOffset UpdatedAt,
    IReadOnlyDictionary<string, string> Metadata);

public sealed record ConnectorPage<T>(
    IReadOnlyList<T> Items,
    int Offset,
    int Limit,
    int TotalCount);

public interface IConnectorHandler
{
    string ConnectorId { get; }

    ValueTask<ConnectorExecutionOutcome> ExecuteAsync(
        ConnectorExecutionRequest request,
        CancellationToken cancellationToken);
}

public interface IConnectorHealthProbe
{
    string ConnectorId { get; }

    ValueTask<ConnectorHealthSnapshot> CheckAsync(CancellationToken cancellationToken);
}

public interface IConnectorAuditSink
{
    ValueTask PublishAsync(ConnectorAuditRecord record, CancellationToken cancellationToken);
}

public interface IConnectorCheckpointStore
{
    ValueTask<ConnectorCheckpoint?> GetAsync(
        string connectorId,
        string partition,
        CancellationToken cancellationToken);

    ValueTask<bool> SaveAsync(
        ConnectorCheckpoint checkpoint,
        long expectedRevision,
        CancellationToken cancellationToken);
}

public static class ConnectorContract
{
    public static IReadOnlyDictionary<string, string> EmptyDetails { get; } =
        new ReadOnlyDictionary<string, string>(
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));
}
