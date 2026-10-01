namespace AtlasOps.Features.Storage.StorageLifecycleMonitoring;

public sealed record UpdateStorageLifecycleMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);