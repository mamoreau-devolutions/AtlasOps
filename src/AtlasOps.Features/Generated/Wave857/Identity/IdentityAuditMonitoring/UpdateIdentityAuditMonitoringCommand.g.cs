namespace AtlasOps.Features.Identity.IdentityAuditMonitoring;

public sealed record UpdateIdentityAuditMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);