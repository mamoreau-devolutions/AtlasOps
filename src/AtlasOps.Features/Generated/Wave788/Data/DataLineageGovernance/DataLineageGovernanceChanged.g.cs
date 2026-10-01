namespace AtlasOps.Features.Data.DataLineageGovernance;

public sealed record DataLineageGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);