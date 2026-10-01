namespace AtlasOps.Features.Compute.ComputeLifecycleOptimization;

public sealed record UpdateComputeLifecycleOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);