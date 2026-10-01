namespace AtlasOps.Features.Architecture.ArchitectureExceptionGovernance;

public sealed record UpdateArchitectureExceptionGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);