namespace AtlasOps.Features.Security.SecurityBoundaryGovernance;

public sealed record SecurityBoundaryGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);