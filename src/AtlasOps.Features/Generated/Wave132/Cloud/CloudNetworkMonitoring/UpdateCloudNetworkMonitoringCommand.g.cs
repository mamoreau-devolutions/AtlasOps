namespace AtlasOps.Features.Cloud.CloudNetworkMonitoring;

public sealed record UpdateCloudNetworkMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);