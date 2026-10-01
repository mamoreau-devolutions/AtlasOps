namespace AtlasOps.Features.Identity.IdentityLifecycleRecovery;

public sealed record UpdateIdentityLifecycleRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);