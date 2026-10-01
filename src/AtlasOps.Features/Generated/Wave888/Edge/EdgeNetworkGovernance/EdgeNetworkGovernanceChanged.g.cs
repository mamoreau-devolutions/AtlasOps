namespace AtlasOps.Features.Edge.EdgeNetworkGovernance;

public sealed record EdgeNetworkGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);