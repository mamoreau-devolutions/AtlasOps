namespace AtlasOps.Features.Observability.LogQueryMonitoring;

public sealed record LogQueryMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);