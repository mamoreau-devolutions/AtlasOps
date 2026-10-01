namespace AtlasOps.Features.Identity.IdentityProviderRecovery;

public sealed record UpdateIdentityProviderRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);