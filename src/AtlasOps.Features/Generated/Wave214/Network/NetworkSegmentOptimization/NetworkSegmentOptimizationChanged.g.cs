namespace AtlasOps.Features.Network.NetworkSegmentOptimization;

public sealed record NetworkSegmentOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);