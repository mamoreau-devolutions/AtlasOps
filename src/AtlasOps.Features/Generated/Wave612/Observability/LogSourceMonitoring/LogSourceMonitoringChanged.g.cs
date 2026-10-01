namespace AtlasOps.Features.Observability.LogSourceMonitoring;

public sealed record LogSourceMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);