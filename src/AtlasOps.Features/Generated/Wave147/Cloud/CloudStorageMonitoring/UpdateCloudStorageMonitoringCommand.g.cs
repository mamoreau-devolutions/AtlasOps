namespace AtlasOps.Features.Cloud.CloudStorageMonitoring;

public sealed record UpdateCloudStorageMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);