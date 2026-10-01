namespace AtlasOps.Features.Security.SecurityIdentityGovernance;

public sealed record UpdateSecurityIdentityGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);