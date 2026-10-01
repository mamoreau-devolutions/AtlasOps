namespace AtlasOps.Features.FinOps.BudgetPlanGovernance;

public sealed record UpdateBudgetPlanGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);