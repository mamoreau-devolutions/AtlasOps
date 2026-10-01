namespace AtlasOps.Features.Architecture.ArchitectureComponentOptimization;

public sealed record UpdateArchitectureComponentOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);