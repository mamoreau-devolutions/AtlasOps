namespace AtlasOps.Features.Architecture.ArchitectureEvidenceGovernance;

public sealed record UpdateArchitectureEvidenceGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);