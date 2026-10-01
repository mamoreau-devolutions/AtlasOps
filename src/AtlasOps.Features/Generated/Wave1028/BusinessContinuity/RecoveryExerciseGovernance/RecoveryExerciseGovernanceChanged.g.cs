namespace AtlasOps.Features.BusinessContinuity.RecoveryExerciseGovernance;

public sealed record RecoveryExerciseGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);