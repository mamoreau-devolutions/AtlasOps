namespace AtlasOps.Features.FinOps.CostCenterMonitoring;

public sealed record CostCenterMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);