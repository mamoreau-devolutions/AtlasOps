namespace AtlasOps.Features.Storage.StorageReplicationMonitoring;

public sealed record UpdateStorageReplicationMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);