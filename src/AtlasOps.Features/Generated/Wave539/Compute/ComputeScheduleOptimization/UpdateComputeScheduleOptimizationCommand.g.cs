namespace AtlasOps.Features.Compute.ComputeScheduleOptimization;

public sealed record UpdateComputeScheduleOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);