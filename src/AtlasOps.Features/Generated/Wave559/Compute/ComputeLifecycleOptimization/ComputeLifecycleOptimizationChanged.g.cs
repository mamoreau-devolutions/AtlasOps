namespace AtlasOps.Features.Compute.ComputeLifecycleOptimization;

public sealed record ComputeLifecycleOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);