namespace AtlasOps.Features.FinOps.ResourceCommitmentGovernance;

public sealed record UpdateResourceCommitmentGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);