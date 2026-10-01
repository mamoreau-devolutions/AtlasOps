namespace AtlasOps.Features.FinOps.BudgetPlanMonitoring;

public sealed record BudgetPlanMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);