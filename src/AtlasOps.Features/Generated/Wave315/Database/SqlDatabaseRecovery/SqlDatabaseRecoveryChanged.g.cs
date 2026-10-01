namespace AtlasOps.Features.Database.SqlDatabaseRecovery;

public sealed record SqlDatabaseRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);