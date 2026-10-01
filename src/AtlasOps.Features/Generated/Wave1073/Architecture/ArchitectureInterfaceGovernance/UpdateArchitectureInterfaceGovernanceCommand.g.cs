namespace AtlasOps.Features.Architecture.ArchitectureInterfaceGovernance;

public sealed record UpdateArchitectureInterfaceGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);