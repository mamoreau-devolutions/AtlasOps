namespace AtlasOps.Features.Delivery.ReleasePipelineOptimization;

public sealed record ReleasePipelineOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);