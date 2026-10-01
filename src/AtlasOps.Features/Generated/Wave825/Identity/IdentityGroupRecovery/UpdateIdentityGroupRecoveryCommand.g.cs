namespace AtlasOps.Features.Identity.IdentityGroupRecovery;

public sealed record UpdateIdentityGroupRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);