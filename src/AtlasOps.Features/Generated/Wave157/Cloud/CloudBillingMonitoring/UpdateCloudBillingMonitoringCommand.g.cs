namespace AtlasOps.Features.Cloud.CloudBillingMonitoring;

public sealed record UpdateCloudBillingMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);