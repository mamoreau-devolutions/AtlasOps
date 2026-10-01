namespace AtlasOps.Features.Security.SecurityExceptionGovernance;

public sealed record SecurityExceptionGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);