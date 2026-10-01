namespace AtlasOps.Features.Api.ApiTokenGovernance;

public sealed record ApiTokenGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);