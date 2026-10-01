namespace AtlasOps.Features.Security.SecurityKeyMonitoring;

public sealed record UpdateSecurityKeyMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);