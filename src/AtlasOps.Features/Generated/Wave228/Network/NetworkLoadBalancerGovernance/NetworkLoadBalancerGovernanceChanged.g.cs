namespace AtlasOps.Features.Network.NetworkLoadBalancerGovernance;

public sealed record NetworkLoadBalancerGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);