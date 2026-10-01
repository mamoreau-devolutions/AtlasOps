namespace AtlasOps.Features.Identity.IdentitySessionRecovery;

public sealed record UpdateIdentitySessionRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);