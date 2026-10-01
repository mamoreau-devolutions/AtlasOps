namespace AtlasOps.Features.Identity.IdentityUserMonitoring;

public sealed record UpdateIdentityUserMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);