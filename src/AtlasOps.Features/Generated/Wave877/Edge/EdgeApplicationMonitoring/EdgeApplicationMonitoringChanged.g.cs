namespace AtlasOps.Features.Edge.EdgeApplicationMonitoring;

public sealed record EdgeApplicationMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);