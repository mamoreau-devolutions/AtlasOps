namespace AtlasOps.Features.FinOps.CostAllocationOptimization;

public sealed record UpdateCostAllocationOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);