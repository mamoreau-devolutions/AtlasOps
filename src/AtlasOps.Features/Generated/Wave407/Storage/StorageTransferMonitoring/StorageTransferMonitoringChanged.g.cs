namespace AtlasOps.Features.Storage.StorageTransferMonitoring;

public sealed record StorageTransferMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);