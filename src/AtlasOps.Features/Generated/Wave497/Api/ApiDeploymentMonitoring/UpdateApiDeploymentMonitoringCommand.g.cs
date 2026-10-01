namespace AtlasOps.Features.Api.ApiDeploymentMonitoring;

public sealed record UpdateApiDeploymentMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);