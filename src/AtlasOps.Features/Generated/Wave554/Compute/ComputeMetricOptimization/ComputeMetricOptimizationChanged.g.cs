namespace AtlasOps.Features.Compute.ComputeMetricOptimization;

public sealed record ComputeMetricOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);