namespace AtlasOps.Features.FinOps.CostAnomalyMonitoring;

public sealed record CostAnomalyMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);