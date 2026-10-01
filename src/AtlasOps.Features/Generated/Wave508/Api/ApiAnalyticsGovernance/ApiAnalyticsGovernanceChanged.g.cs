namespace AtlasOps.Features.Api.ApiAnalyticsGovernance;

public sealed record ApiAnalyticsGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);