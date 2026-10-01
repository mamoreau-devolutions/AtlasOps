namespace AtlasOps.Features.Storage.StorageArchiveMonitoring;

public sealed record StorageArchiveMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);