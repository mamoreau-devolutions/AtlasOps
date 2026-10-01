namespace AtlasOps.Features.FinOps.CostAnomalyOptimization;

public sealed record UpdateCostAnomalyOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);