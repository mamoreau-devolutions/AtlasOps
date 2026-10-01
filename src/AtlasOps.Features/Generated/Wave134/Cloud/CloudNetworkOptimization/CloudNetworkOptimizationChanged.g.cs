namespace AtlasOps.Features.Cloud.CloudNetworkOptimization;

public sealed record CloudNetworkOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);