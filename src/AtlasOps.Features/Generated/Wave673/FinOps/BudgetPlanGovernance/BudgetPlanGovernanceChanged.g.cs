namespace AtlasOps.Features.FinOps.BudgetPlanGovernance;

public sealed record BudgetPlanGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);