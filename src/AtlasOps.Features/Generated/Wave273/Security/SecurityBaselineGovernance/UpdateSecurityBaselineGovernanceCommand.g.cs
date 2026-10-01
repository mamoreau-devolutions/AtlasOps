namespace AtlasOps.Features.Security.SecurityBaselineGovernance;

public sealed record UpdateSecurityBaselineGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);