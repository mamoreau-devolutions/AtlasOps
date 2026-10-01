namespace AtlasOps.Features.BusinessContinuity.RecoveryObjectiveRecovery;

public sealed record RecoveryObjectiveRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);