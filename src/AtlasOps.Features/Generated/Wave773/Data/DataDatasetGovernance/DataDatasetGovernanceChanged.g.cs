namespace AtlasOps.Features.Data.DataDatasetGovernance;

public sealed record DataDatasetGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);