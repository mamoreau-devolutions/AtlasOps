namespace AtlasOps.Features.BusinessContinuity.RecoveryExerciseGovernance;

public sealed record UpdateRecoveryExerciseGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);