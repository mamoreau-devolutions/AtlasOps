namespace AtlasOps.Features.Architecture.ArchitectureRoadmapRecovery;

public sealed record UpdateArchitectureRoadmapRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);