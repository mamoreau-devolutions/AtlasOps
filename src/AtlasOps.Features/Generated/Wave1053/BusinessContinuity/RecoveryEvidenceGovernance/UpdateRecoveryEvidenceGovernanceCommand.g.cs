namespace AtlasOps.Features.BusinessContinuity.RecoveryEvidenceGovernance;

public sealed record UpdateRecoveryEvidenceGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);