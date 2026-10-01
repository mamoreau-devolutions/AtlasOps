namespace AtlasOps.Features.BusinessContinuity.RecoveryReviewProvisioning;

public sealed record RecoveryReviewProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);