namespace AtlasOps.Features.Security.SecurityBoundaryMonitoring;

public sealed record UpdateSecurityBoundaryMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);