namespace AtlasOps.Features.Api.ApiContractGovernance;

public sealed record ApiContractGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);