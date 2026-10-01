namespace AtlasOps.Features.Delivery.BuildArtifactOptimization;

public sealed record UpdateBuildArtifactOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);