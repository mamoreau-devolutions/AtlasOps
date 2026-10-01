namespace AtlasOps.Features.Identity.IdentityGroupGovernance;

public sealed record UpdateIdentityGroupGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);