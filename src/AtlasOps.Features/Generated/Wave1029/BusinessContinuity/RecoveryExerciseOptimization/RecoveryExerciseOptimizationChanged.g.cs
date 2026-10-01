namespace AtlasOps.Features.BusinessContinuity.RecoveryExerciseOptimization;

public sealed record RecoveryExerciseOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);