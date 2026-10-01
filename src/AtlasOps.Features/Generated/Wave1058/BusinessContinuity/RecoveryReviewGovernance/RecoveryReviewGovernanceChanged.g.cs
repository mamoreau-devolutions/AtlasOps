namespace AtlasOps.Features.BusinessContinuity.RecoveryReviewGovernance;

public sealed record RecoveryReviewGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);