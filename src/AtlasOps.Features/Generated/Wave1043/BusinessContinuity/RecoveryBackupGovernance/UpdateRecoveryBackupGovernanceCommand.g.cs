namespace AtlasOps.Features.BusinessContinuity.RecoveryBackupGovernance;

public sealed record UpdateRecoveryBackupGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);