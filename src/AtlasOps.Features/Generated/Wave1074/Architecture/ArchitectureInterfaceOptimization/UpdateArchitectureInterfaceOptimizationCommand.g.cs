namespace AtlasOps.Features.Architecture.ArchitectureInterfaceOptimization;

public sealed record UpdateArchitectureInterfaceOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);