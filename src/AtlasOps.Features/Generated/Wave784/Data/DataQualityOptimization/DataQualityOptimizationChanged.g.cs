namespace AtlasOps.Features.Data.DataQualityOptimization;

public sealed record DataQualityOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);