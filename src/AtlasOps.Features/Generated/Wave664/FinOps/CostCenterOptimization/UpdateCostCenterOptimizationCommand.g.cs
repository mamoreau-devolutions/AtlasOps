namespace AtlasOps.Features.FinOps.CostCenterOptimization;

public sealed record UpdateCostCenterOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);