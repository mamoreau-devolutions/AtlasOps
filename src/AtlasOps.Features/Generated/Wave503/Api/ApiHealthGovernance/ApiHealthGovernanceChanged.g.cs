namespace AtlasOps.Features.Api.ApiHealthGovernance;

public sealed record ApiHealthGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);