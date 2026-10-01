namespace AtlasOps.Features.Edge.EdgeIncidentRecovery;

public sealed record EdgeIncidentRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);