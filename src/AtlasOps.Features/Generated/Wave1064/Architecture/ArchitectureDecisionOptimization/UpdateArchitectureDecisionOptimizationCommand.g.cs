namespace AtlasOps.Features.Architecture.ArchitectureDecisionOptimization;

public sealed record UpdateArchitectureDecisionOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);