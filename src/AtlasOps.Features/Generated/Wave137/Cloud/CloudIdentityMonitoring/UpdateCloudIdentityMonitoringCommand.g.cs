namespace AtlasOps.Features.Cloud.CloudIdentityMonitoring;

public sealed record UpdateCloudIdentityMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);