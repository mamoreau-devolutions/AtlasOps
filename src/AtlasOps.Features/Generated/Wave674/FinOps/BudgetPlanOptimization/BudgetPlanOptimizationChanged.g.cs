namespace AtlasOps.Features.FinOps.BudgetPlanOptimization;

public sealed record BudgetPlanOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);