namespace AtlasOps.Features.Api.ApiEndpointMonitoring;

public sealed record UpdateApiEndpointMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);