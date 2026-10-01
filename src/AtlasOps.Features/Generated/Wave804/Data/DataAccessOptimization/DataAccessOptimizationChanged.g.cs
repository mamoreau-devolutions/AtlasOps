namespace AtlasOps.Features.Data.DataAccessOptimization;

public sealed record DataAccessOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);