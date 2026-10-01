namespace AtlasOps.Features.Architecture.ArchitectureDecisionGovernance;

public sealed record UpdateArchitectureDecisionGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);