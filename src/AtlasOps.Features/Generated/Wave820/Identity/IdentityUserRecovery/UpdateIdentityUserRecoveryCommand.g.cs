namespace AtlasOps.Features.Identity.IdentityUserRecovery;

public sealed record UpdateIdentityUserRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);