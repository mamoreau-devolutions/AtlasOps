namespace AtlasOps.Features.Delivery.ReleaseApprovalRecovery;

public sealed record ReleaseApprovalRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);