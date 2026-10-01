namespace AtlasOps.Features.Api.ApiVersionMonitoring;

public sealed record UpdateApiVersionMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);