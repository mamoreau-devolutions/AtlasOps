namespace AtlasOps.Features.Compute.ComputeTemplateOptimization;

public sealed record ComputeTemplateOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);