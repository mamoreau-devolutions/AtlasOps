namespace AtlasOps.Features.Compute.ComputeImageOptimization;

public sealed record UpdateComputeImageOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);