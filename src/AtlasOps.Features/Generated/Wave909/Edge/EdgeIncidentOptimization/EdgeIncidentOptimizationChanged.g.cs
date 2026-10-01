namespace AtlasOps.Features.Edge.EdgeIncidentOptimization;

public sealed record EdgeIncidentOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);