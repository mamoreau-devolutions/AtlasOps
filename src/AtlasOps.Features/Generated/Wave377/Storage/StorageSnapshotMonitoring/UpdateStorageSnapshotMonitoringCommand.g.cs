namespace AtlasOps.Features.Storage.StorageSnapshotMonitoring;

public sealed record UpdateStorageSnapshotMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);