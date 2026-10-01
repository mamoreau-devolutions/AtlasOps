namespace AtlasOps.Features.FinOps.CostCenterGovernance;

public sealed record UpdateCostCenterGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);