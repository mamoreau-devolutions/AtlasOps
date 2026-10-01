namespace AtlasOps.Features.Database.DatabaseBackupOptimization;

public sealed record DatabaseBackupOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);