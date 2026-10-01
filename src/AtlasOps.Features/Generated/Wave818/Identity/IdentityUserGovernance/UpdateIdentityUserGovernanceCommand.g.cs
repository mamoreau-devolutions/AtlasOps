namespace AtlasOps.Features.Identity.IdentityUserGovernance;

public sealed record UpdateIdentityUserGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);