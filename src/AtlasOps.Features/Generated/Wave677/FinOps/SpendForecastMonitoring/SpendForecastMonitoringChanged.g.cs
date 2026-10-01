namespace AtlasOps.Features.FinOps.SpendForecastMonitoring;

public sealed record SpendForecastMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);