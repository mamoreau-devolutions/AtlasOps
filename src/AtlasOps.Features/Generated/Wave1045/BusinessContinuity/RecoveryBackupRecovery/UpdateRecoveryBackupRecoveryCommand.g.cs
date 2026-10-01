namespace AtlasOps.Features.BusinessContinuity.RecoveryBackupRecovery;

public sealed record UpdateRecoveryBackupRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);