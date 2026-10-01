namespace AtlasOps.Features.Storage.StorageLifecycleMonitoring;

public sealed record StorageLifecycleMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);