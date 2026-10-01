namespace AtlasOps.Features.ServiceManagement.ServiceReviewRecovery;

public sealed record ServiceReviewRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);