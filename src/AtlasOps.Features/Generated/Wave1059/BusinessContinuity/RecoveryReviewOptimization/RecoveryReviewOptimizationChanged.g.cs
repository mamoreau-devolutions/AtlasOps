namespace AtlasOps.Features.BusinessContinuity.RecoveryReviewOptimization;

public sealed record RecoveryReviewOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);