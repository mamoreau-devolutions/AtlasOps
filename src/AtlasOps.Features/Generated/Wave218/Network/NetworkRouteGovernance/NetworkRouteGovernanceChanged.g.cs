namespace AtlasOps.Features.Network.NetworkRouteGovernance;

public sealed record NetworkRouteGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);