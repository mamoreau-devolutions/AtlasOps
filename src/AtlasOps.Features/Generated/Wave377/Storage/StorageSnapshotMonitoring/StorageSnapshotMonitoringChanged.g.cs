namespace AtlasOps.Features.Storage.StorageSnapshotMonitoring;

public sealed record StorageSnapshotMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);