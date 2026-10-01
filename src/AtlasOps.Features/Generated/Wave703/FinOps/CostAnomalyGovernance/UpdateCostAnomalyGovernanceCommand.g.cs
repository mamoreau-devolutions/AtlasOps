namespace AtlasOps.Features.FinOps.CostAnomalyGovernance;

public sealed record UpdateCostAnomalyGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);