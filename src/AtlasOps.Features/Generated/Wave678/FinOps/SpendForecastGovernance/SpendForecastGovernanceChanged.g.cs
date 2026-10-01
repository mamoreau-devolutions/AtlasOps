namespace AtlasOps.Features.FinOps.SpendForecastGovernance;

public sealed record SpendForecastGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);