namespace AtlasOps.Features.Observability.TraceSourceMonitoring;

public sealed record TraceSourceMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);