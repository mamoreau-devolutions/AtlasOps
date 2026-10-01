namespace AtlasOps.Features.Compute.ComputeMetricOptimization;

public sealed record UpdateComputeMetricOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);