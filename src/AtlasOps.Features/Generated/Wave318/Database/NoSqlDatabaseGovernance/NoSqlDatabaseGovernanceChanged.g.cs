namespace AtlasOps.Features.Database.NoSqlDatabaseGovernance;

public sealed record NoSqlDatabaseGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);