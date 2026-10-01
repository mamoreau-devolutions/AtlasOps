namespace AtlasOps.Features.Database.NoSqlDatabaseOptimization;

public sealed record NoSqlDatabaseOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);