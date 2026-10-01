namespace AtlasOps.Features.Database.SqlDatabaseOptimization;

public sealed record SqlDatabaseOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);