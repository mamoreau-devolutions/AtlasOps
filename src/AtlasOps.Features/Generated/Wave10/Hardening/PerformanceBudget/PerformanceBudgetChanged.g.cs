namespace AtlasOps.Features.Hardening.PerformanceBudget;

public sealed record PerformanceBudgetChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);