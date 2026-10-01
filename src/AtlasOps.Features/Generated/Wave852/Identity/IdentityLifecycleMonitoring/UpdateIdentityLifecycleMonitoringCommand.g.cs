namespace AtlasOps.Features.Identity.IdentityLifecycleMonitoring;

public sealed record UpdateIdentityLifecycleMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);