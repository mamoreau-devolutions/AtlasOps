namespace AtlasOps.Features.Storage.StorageArchiveMonitoring;

public sealed record UpdateStorageArchiveMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);