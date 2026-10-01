namespace AtlasOps.Features.Cloud.AwsAccountMonitoring;

public sealed record UpdateAwsAccountMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);