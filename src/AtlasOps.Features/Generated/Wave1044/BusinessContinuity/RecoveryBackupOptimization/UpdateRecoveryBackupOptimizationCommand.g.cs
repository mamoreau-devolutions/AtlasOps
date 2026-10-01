namespace AtlasOps.Features.BusinessContinuity.RecoveryBackupOptimization;

public sealed record UpdateRecoveryBackupOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);