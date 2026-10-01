namespace AtlasOps.Features.BusinessContinuity.RecoveryExerciseMonitoring;

public sealed record UpdateRecoveryExerciseMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);