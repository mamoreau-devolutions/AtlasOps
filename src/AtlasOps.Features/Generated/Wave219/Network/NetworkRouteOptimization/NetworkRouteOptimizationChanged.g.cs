namespace AtlasOps.Features.Network.NetworkRouteOptimization;

public sealed record NetworkRouteOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);