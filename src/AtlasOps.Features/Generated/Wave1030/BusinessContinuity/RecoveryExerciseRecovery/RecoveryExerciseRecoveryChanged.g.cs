namespace AtlasOps.Features.BusinessContinuity.RecoveryExerciseRecovery;

public sealed record RecoveryExerciseRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);