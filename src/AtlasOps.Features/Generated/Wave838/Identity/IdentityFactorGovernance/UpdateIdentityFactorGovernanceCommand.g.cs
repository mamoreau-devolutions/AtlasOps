namespace AtlasOps.Features.Identity.IdentityFactorGovernance;

public sealed record UpdateIdentityFactorGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);