namespace AtlasOps.Features.Network.NetworkLoadBalancerRecovery;

public sealed record NetworkLoadBalancerRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);