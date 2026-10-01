namespace AtlasOps.Features.Security.SecuritySessionMonitoring;

public sealed record UpdateSecuritySessionMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);