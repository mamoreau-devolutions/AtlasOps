namespace AtlasOps.Features.Cloud.CloudDatabaseMonitoring;

public sealed record UpdateCloudDatabaseMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);