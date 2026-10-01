namespace AtlasOps.Features.Delivery.ReleaseApprovalGovernance;

public sealed record ReleaseApprovalGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);