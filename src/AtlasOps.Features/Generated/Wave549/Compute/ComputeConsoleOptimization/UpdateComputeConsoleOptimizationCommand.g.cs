namespace AtlasOps.Features.Compute.ComputeConsoleOptimization;

public sealed record UpdateComputeConsoleOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);