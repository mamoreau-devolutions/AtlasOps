namespace AtlasOps.Features.Identity.IdentityFactorMonitoring;

public sealed record UpdateIdentityFactorMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);