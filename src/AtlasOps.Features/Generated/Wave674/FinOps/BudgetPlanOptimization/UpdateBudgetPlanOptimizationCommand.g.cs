namespace AtlasOps.Features.FinOps.BudgetPlanOptimization;

public sealed record UpdateBudgetPlanOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);