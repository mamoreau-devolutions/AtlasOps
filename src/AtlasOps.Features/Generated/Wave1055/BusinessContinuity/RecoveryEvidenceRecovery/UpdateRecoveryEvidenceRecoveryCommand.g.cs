namespace AtlasOps.Features.BusinessContinuity.RecoveryEvidenceRecovery;

public sealed record UpdateRecoveryEvidenceRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);