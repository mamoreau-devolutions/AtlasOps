namespace AtlasOps.Features.Observability.MetricSourceMonitoring;

public sealed record MetricSourceMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);