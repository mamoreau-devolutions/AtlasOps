namespace AtlasOps.Features.Delivery.BuildPipelineOptimization;

public sealed record UpdateBuildPipelineOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);