namespace AtlasOps.Features.FinOps.BudgetPlanRecovery;

public sealed record BudgetPlanRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);