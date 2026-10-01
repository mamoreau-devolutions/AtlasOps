namespace AtlasOps.Features.Database.NoSqlDatabaseRecovery;

public sealed record NoSqlDatabaseRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);