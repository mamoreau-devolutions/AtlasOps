namespace AtlasOps.Features.BusinessContinuity.ContinuityPlanGovernance;

public sealed record ContinuityPlanGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);