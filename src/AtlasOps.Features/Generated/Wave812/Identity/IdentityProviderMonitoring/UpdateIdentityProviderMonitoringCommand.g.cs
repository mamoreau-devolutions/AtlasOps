namespace AtlasOps.Features.Identity.IdentityProviderMonitoring;

public sealed record UpdateIdentityProviderMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);