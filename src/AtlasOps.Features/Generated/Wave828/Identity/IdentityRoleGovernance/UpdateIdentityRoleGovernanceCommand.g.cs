namespace AtlasOps.Features.Identity.IdentityRoleGovernance;

public sealed record UpdateIdentityRoleGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);