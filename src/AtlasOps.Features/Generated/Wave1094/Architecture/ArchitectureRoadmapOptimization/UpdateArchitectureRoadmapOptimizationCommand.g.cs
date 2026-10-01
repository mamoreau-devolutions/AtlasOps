namespace AtlasOps.Features.Architecture.ArchitectureRoadmapOptimization;

public sealed record UpdateArchitectureRoadmapOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);