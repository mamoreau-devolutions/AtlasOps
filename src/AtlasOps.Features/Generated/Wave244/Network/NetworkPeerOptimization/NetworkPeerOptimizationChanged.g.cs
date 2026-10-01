namespace AtlasOps.Features.Network.NetworkPeerOptimization;

public sealed record NetworkPeerOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);