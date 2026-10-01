namespace AtlasOps.Features.Identity.IdentityProviderGovernance;

public sealed record UpdateIdentityProviderGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);