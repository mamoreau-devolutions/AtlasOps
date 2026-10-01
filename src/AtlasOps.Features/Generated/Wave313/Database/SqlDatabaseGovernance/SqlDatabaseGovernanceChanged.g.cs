namespace AtlasOps.Features.Database.SqlDatabaseGovernance;

public sealed record SqlDatabaseGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);