namespace AtlasOps.Features.Compute.ComputeScheduleOptimization;

public sealed record ComputeScheduleOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);