namespace AtlasOps.Features.Identity.IdentityGroupMonitoring;

public sealed record UpdateIdentityGroupMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);