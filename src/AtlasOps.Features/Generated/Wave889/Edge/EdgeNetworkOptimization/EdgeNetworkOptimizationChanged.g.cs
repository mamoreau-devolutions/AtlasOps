namespace AtlasOps.Features.Edge.EdgeNetworkOptimization;

public sealed record EdgeNetworkOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);