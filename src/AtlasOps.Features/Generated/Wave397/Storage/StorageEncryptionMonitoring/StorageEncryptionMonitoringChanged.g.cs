namespace AtlasOps.Features.Storage.StorageEncryptionMonitoring;

public sealed record StorageEncryptionMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);