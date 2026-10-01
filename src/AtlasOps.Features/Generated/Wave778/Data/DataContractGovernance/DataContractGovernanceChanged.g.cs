namespace AtlasOps.Features.Data.DataContractGovernance;

public sealed record DataContractGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);