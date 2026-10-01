namespace AtlasOps.Features.Delivery.BuildArtifactOptimization;

public sealed record BuildArtifactOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);