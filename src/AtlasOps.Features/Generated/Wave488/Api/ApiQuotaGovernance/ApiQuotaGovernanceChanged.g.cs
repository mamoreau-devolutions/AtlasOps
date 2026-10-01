namespace AtlasOps.Features.Api.ApiQuotaGovernance;

public sealed record ApiQuotaGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);