namespace AtlasOps.Features.Edge.EdgeUpdateGovernance;

public sealed record EdgeUpdateGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);