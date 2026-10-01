namespace AtlasOps.Features.BusinessContinuity.RecoveryExerciseOptimization;

public sealed record UpdateRecoveryExerciseOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);