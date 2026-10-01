namespace AtlasOps.Features.Compute.ComputeScaleSetOptimization;

public sealed record UpdateComputeScaleSetOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);