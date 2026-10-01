namespace AtlasOps.Features.Database.DatabaseRestoreOptimization;

public sealed record DatabaseRestoreOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);