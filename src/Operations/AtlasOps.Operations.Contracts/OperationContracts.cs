namespace AtlasOps.Operations.Contracts;

public enum DurableJobStatus
{
    Pending,
    Leased,
    Running,
    WaitingForRetry,
    Succeeded,
    Failed,
    Cancelled,
    DeadLettered,
}

public enum OperationFailureKind
{
    None,
    Validation,
    Authentication,
    Authorization,
    Throttled,
    Conflict,
    Transient,
    Permanent,
    Cancelled,
}

public enum SynchronizationDirection
{
    Import,
    Export,
    Bidirectional,
}

public sealed record OperationDescriptor(
    string Id,
    string DisplayName,
    string ProviderFamily,
    bool SupportsPaging,
    bool SupportsIncrementalSync,
    int MaximumConcurrency,
    int MaximumAttempts,
    TimeSpan Timeout);

public sealed record OperationEnvelope(
    Guid Id,
    string ProviderId,
    string Operation,
    DateTimeOffset CreatedAt,
    string? CredentialReference,
    IReadOnlyDictionary<string, string> Parameters);

public sealed record OperationExecutionResult(
    Guid OperationId,
    bool Succeeded,
    OperationFailureKind FailureKind,
    string Diagnostic,
    string? ContinuationToken,
    IReadOnlyDictionary<string, string> Details);

public sealed record DurableJob(
    Guid Id,
    OperationEnvelope Envelope,
    DurableJobStatus Status,
    int Attempt,
    int MaximumAttempts,
    DateTimeOffset AvailableAt,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    long Revision,
    string? LastDiagnostic);

public sealed record OperationLease(
    Guid JobId,
    string Owner,
    DateTimeOffset AcquiredAt,
    DateTimeOffset ExpiresAt,
    long FencingToken);

public sealed record InboxMessage(
    string ProviderId,
    string MessageId,
    string ContentType,
    byte[] Payload,
    DateTimeOffset ReceivedAt,
    string ContentHash);

public sealed record OutboxMessage(
    Guid Id,
    string Destination,
    string ContentType,
    byte[] Payload,
    DateTimeOffset CreatedAt,
    DateTimeOffset? PublishedAt,
    int Attempt,
    string? LastDiagnostic);

public sealed record SynchronizationCheckpoint(
    string ProviderId,
    string Scope,
    string Cursor,
    DateTimeOffset UpdatedAt,
    long Revision);

public sealed record SynchronizationConflict(
    Guid Id,
    string ProviderId,
    string ResourceType,
    string ResourceId,
    string LocalRevision,
    string RemoteRevision,
    DateTimeOffset DetectedAt,
    IReadOnlyDictionary<string, string> LocalValues,
    IReadOnlyDictionary<string, string> RemoteValues);

public sealed record OperationPage<T>(
    IReadOnlyList<T> Items,
    string? ContinuationToken,
    bool HasMore);

public sealed record JobMutationResult(
    bool Succeeded,
    string Diagnostic,
    DurableJob? Job);

public sealed record LeaseAcquisitionResult(
    bool Acquired,
    string Diagnostic,
    OperationLease? Lease);

public sealed record InboxAcceptanceResult(
    bool Accepted,
    bool Duplicate,
    string Diagnostic);

public sealed record CheckpointMutationResult(
    bool Succeeded,
    string Diagnostic,
    SynchronizationCheckpoint? Checkpoint);

public static class OperationContract
{
    public static IReadOnlyDictionary<string, string> EmptyDetails { get; } =
        new Dictionary<string, string>(StringComparer.Ordinal);
}

public interface IOperationHandler
{
    string OperationId { get; }

    ValueTask<OperationExecutionResult> ExecuteAsync(
        OperationEnvelope envelope,
        CancellationToken cancellationToken);
}

public interface IDurableJobStore
{
    ValueTask<JobMutationResult> EnqueueAsync(DurableJob job, CancellationToken cancellationToken);

    ValueTask<IReadOnlyList<DurableJob>> GetAvailableAsync(
        DateTimeOffset now,
        int maximumCount,
        CancellationToken cancellationToken);

    ValueTask<JobMutationResult> MutateAsync(
        Guid jobId,
        long expectedRevision,
        DurableJobStatus nextStatus,
        DateTimeOffset availableAt,
        string? diagnostic,
        CancellationToken cancellationToken);
}

public interface ICheckpointStore
{
    ValueTask<SynchronizationCheckpoint?> GetAsync(
        string providerId,
        string scope,
        CancellationToken cancellationToken);

    ValueTask<CheckpointMutationResult> WriteAsync(
        SynchronizationCheckpoint checkpoint,
        long expectedRevision,
        CancellationToken cancellationToken);
}
