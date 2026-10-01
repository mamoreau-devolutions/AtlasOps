namespace AtlasOps.Features.Storage.StorageReplicationMonitoring;

public sealed record StorageReplicationMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);