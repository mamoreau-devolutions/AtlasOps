namespace AtlasOps.Features.Security.SecurityIdentityMonitoring;

public sealed record UpdateSecurityIdentityMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);