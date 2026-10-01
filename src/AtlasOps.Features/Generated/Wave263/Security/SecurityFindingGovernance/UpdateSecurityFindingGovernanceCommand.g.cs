namespace AtlasOps.Features.Security.SecurityFindingGovernance;

public sealed record UpdateSecurityFindingGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);