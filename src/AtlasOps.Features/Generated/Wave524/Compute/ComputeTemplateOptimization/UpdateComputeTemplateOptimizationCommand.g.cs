namespace AtlasOps.Features.Compute.ComputeTemplateOptimization;

public sealed record UpdateComputeTemplateOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);