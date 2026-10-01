namespace AtlasOps.Features.Api.ApiVersionGovernance;

public sealed record ApiVersionGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);