namespace AtlasOps.Features.Data.DataSourceGovernance;

public sealed record DataSourceGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);