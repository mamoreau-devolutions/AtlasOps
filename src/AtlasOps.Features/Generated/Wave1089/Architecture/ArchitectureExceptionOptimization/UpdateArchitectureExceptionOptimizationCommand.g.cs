namespace AtlasOps.Features.Architecture.ArchitectureExceptionOptimization;

public sealed record UpdateArchitectureExceptionOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);