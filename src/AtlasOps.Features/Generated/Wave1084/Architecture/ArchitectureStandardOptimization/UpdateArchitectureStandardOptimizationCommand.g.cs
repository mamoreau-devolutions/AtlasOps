namespace AtlasOps.Features.Architecture.ArchitectureStandardOptimization;

public sealed record UpdateArchitectureStandardOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);