namespace AtlasOps.Features.FinOps.CostAllocationGovernance;

public sealed record UpdateCostAllocationGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);