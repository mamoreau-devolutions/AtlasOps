namespace AtlasOps.Features.Hardening.MemoryBudget;

public sealed record MemoryBudgetChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);