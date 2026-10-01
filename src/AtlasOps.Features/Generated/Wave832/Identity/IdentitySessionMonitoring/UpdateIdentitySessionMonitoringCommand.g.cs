namespace AtlasOps.Features.Identity.IdentitySessionMonitoring;

public sealed record UpdateIdentitySessionMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);