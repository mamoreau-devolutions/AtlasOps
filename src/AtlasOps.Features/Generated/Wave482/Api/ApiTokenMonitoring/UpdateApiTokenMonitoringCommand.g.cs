namespace AtlasOps.Features.Api.ApiTokenMonitoring;

public sealed record UpdateApiTokenMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);