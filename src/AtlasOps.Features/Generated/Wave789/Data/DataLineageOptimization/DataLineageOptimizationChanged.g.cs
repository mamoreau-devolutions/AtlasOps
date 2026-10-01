namespace AtlasOps.Features.Data.DataLineageOptimization;

public sealed record DataLineageOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);