namespace AtlasOps.Features.Identity.IdentitySessionGovernance;

public sealed record UpdateIdentitySessionGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);