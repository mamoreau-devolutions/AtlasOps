namespace AtlasOps.Features.Api.ApiClientGovernance;

public sealed record ApiClientGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);