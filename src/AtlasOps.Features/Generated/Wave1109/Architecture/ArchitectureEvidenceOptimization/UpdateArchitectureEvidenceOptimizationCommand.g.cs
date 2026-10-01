namespace AtlasOps.Features.Architecture.ArchitectureEvidenceOptimization;

public sealed record UpdateArchitectureEvidenceOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);