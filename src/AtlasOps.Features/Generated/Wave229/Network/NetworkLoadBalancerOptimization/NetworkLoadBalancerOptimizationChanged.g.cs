namespace AtlasOps.Features.Network.NetworkLoadBalancerOptimization;

public sealed record NetworkLoadBalancerOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);