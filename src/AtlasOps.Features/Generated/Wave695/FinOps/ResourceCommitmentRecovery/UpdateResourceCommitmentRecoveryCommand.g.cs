namespace AtlasOps.Features.FinOps.ResourceCommitmentRecovery;

public sealed record UpdateResourceCommitmentRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);