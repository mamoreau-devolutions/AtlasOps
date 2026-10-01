namespace AtlasOps.Features.Incidents.ErrorBudget;

public sealed record ErrorBudgetChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);