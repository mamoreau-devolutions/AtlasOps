namespace AtlasOps.Features.Architecture.ArchitectureRiskOptimization;

public sealed record UpdateArchitectureRiskOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);