namespace AtlasOps.Features.Identity.IdentityRoleMonitoring;

public sealed record UpdateIdentityRoleMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);