namespace AtlasOps.Features.Identity.IdentityClaimRecovery;

public sealed record UpdateIdentityClaimRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);