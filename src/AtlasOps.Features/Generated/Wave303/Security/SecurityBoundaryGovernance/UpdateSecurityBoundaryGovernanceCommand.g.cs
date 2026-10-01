namespace AtlasOps.Features.Security.SecurityBoundaryGovernance;

public sealed record UpdateSecurityBoundaryGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);