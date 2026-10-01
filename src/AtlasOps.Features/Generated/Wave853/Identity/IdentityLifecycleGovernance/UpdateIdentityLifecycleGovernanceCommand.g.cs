namespace AtlasOps.Features.Identity.IdentityLifecycleGovernance;

public sealed record UpdateIdentityLifecycleGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);