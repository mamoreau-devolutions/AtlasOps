namespace AtlasOps.Features.Architecture.ArchitectureEvidenceRecovery;

public sealed record UpdateArchitectureEvidenceRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);