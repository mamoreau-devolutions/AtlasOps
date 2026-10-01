namespace AtlasOps.Features.Delivery.BuildPipelineOptimization;

public sealed record BuildPipelineOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);