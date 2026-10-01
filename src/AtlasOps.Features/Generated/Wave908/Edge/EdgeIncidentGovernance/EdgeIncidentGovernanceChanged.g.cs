namespace AtlasOps.Features.Edge.EdgeIncidentGovernance;

public sealed record EdgeIncidentGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);