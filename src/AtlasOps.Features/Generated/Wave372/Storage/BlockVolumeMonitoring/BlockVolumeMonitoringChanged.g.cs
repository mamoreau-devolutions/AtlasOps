namespace AtlasOps.Features.Storage.BlockVolumeMonitoring;

public sealed record BlockVolumeMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);