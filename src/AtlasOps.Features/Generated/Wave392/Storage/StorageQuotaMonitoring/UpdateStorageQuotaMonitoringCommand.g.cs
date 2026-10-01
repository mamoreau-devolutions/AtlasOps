namespace AtlasOps.Features.Storage.StorageQuotaMonitoring;

public sealed record UpdateStorageQuotaMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);