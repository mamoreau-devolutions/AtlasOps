namespace AtlasOps.Features.Data.DataProductGovernance;

public sealed record DataProductGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);