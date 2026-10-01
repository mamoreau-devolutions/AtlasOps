namespace AtlasOps.Features.Storage.ObjectBucketMonitoring;

public sealed record UpdateObjectBucketMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);