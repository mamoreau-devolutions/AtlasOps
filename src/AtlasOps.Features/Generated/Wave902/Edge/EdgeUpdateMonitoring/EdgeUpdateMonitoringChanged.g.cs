namespace AtlasOps.Features.Edge.EdgeUpdateMonitoring;

public sealed record EdgeUpdateMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);