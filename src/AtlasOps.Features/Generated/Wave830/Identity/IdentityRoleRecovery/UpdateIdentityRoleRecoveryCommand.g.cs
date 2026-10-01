namespace AtlasOps.Features.Identity.IdentityRoleRecovery;

public sealed record UpdateIdentityRoleRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);