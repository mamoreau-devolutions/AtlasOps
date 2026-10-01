namespace AtlasOps.Features.Edge.EdgeSiteMonitoring;

public sealed record EdgeSiteMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);