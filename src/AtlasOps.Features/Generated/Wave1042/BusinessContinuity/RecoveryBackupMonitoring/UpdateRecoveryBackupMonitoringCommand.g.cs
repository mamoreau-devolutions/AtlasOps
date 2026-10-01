namespace AtlasOps.Features.BusinessContinuity.RecoveryBackupMonitoring;

public sealed record UpdateRecoveryBackupMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);