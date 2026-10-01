namespace AtlasOps.Features.Api.ApiHealthMonitoring;

public sealed record UpdateApiHealthMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);