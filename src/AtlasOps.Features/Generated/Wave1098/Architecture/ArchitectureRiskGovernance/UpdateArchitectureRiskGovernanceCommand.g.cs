namespace AtlasOps.Features.Architecture.ArchitectureRiskGovernance;

public sealed record UpdateArchitectureRiskGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);