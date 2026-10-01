namespace AtlasOps.Features.Architecture.ArchitectureReviewOptimization;

public sealed record UpdateArchitectureReviewOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);