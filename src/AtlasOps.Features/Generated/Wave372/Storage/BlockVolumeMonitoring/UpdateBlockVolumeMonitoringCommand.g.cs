namespace AtlasOps.Features.Storage.BlockVolumeMonitoring;

public sealed record UpdateBlockVolumeMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);