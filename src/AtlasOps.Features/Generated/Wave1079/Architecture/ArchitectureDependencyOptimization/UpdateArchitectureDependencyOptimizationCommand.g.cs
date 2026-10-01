namespace AtlasOps.Features.Architecture.ArchitectureDependencyOptimization;

public sealed record UpdateArchitectureDependencyOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);