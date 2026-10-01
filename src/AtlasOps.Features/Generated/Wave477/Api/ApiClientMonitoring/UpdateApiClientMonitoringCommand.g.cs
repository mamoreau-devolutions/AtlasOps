namespace AtlasOps.Features.Api.ApiClientMonitoring;

public sealed record UpdateApiClientMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);