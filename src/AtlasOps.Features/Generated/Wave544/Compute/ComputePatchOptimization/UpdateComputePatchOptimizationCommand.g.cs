namespace AtlasOps.Features.Compute.ComputePatchOptimization;

public sealed record UpdateComputePatchOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);