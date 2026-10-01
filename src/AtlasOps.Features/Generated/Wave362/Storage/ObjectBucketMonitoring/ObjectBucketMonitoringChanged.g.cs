namespace AtlasOps.Features.Storage.ObjectBucketMonitoring;

public sealed record ObjectBucketMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);