namespace AtlasOps.Features.Storage.StorageTransferMonitoring;

public sealed record UpdateStorageTransferMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);