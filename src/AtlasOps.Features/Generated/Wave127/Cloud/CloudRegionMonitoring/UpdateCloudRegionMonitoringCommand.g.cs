namespace AtlasOps.Features.Cloud.CloudRegionMonitoring;

public sealed record UpdateCloudRegionMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);