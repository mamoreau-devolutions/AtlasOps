namespace AtlasOps.Features.Identity.IdentityFactorRecovery;

public sealed record UpdateIdentityFactorRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);