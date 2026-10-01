namespace AtlasOps.Features.Edge.EdgeIncidentMonitoring;

public sealed record EdgeIncidentMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);