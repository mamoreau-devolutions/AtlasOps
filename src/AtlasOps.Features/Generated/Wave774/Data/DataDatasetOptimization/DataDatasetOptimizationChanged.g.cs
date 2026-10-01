namespace AtlasOps.Features.Data.DataDatasetOptimization;

public sealed record DataDatasetOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);