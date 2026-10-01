namespace AtlasOps.Features.FinOps.SpendForecastRecovery;

public sealed record SpendForecastRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);