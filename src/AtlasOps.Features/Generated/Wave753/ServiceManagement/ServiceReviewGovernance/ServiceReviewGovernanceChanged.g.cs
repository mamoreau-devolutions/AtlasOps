namespace AtlasOps.Features.ServiceManagement.ServiceReviewGovernance;

public sealed record ServiceReviewGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);