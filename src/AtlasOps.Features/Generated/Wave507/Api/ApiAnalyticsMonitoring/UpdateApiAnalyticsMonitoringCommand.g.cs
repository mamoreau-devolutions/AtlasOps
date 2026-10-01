namespace AtlasOps.Features.Api.ApiAnalyticsMonitoring;

public sealed record UpdateApiAnalyticsMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);