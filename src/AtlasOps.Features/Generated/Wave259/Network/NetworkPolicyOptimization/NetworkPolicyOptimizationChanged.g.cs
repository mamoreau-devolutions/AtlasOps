namespace AtlasOps.Features.Network.NetworkPolicyOptimization;

public sealed record NetworkPolicyOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);