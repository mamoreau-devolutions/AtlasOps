namespace AtlasOps.Features.Identity.IdentityClaimMonitoring;

public sealed record UpdateIdentityClaimMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);