namespace AtlasOps.Features.Identity.IdentityApplicationGovernance;

public sealed record UpdateIdentityApplicationGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);