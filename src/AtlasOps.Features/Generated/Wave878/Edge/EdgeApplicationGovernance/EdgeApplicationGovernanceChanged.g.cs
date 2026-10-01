namespace AtlasOps.Features.Edge.EdgeApplicationGovernance;

public sealed record EdgeApplicationGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);