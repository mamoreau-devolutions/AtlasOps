namespace AtlasOps.Features.Data.DataSourceOptimization;

public sealed record DataSourceOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);