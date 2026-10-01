namespace AtlasOps.Features.Architecture.ArchitectureRoadmapGovernance;

public sealed record UpdateArchitectureRoadmapGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);