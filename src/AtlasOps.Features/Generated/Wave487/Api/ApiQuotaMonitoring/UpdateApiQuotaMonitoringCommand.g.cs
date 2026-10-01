namespace AtlasOps.Features.Api.ApiQuotaMonitoring;

public sealed record UpdateApiQuotaMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);