namespace AtlasOps.Features.Architecture.ArchitectureDependencyGovernance;

public sealed record UpdateArchitectureDependencyGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);