namespace AtlasOps.Features.BusinessContinuity.RecoveryExerciseRecovery;

public sealed record UpdateRecoveryExerciseRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);