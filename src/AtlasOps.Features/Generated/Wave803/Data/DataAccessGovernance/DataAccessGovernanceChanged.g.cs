namespace AtlasOps.Features.Data.DataAccessGovernance;

public sealed record DataAccessGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);