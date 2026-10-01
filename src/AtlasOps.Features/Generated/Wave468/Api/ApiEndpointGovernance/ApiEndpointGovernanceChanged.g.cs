namespace AtlasOps.Features.Api.ApiEndpointGovernance;

public sealed record ApiEndpointGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);