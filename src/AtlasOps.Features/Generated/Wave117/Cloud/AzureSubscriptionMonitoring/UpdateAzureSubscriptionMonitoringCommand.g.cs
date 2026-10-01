namespace AtlasOps.Features.Cloud.AzureSubscriptionMonitoring;

public sealed record UpdateAzureSubscriptionMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);