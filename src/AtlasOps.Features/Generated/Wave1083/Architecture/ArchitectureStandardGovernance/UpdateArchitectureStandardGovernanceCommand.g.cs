namespace AtlasOps.Features.Architecture.ArchitectureStandardGovernance;

public sealed record UpdateArchitectureStandardGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);