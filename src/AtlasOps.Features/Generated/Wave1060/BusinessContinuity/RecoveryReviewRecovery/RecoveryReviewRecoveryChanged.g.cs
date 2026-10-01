namespace AtlasOps.Features.BusinessContinuity.RecoveryReviewRecovery;

public sealed record RecoveryReviewRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);