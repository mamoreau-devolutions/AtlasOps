namespace AtlasOps.Features.Observability.TraceSpanMonitoring;

public sealed record TraceSpanMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);