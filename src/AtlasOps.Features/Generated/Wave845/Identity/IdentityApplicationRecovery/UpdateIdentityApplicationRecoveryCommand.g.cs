namespace AtlasOps.Features.Identity.IdentityApplicationRecovery;

public sealed record UpdateIdentityApplicationRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);