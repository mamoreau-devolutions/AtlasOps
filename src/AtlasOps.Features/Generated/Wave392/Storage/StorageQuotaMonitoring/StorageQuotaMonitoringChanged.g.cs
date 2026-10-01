namespace AtlasOps.Features.Storage.StorageQuotaMonitoring;

public sealed record StorageQuotaMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);