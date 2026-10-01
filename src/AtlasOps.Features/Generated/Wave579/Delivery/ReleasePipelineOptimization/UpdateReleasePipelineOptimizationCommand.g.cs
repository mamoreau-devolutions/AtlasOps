namespace AtlasOps.Features.Delivery.ReleasePipelineOptimization;

public sealed record UpdateReleasePipelineOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);