namespace AtlasOps.Features.Api.ApiContractMonitoring;

public sealed record UpdateApiContractMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);