namespace AtlasOps.Features.FinOps.SpendForecastOptimization;

public sealed record SpendForecastOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);