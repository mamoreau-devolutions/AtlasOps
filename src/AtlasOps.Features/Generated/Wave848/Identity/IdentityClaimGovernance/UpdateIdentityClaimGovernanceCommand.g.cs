namespace AtlasOps.Features.Identity.IdentityClaimGovernance;

public sealed record UpdateIdentityClaimGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);