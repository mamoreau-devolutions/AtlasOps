namespace AtlasOps.Features.Compute.ComputeImageOptimization;

public sealed record ComputeImageOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);