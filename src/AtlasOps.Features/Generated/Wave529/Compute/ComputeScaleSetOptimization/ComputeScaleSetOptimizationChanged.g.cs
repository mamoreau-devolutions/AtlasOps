namespace AtlasOps.Features.Compute.ComputeScaleSetOptimization;

public sealed record ComputeScaleSetOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);