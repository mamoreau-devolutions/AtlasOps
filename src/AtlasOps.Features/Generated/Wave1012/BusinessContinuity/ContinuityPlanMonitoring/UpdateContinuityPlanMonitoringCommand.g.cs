namespace AtlasOps.Features.BusinessContinuity.ContinuityPlanMonitoring;

public sealed record UpdateContinuityPlanMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);