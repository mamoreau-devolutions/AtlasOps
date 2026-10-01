namespace AtlasOps.Features.Security.SecurityFindingMonitoring;

public sealed record UpdateSecurityFindingMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);