namespace AtlasOps.Features.Data.DataTransformOptimization;

public sealed record DataTransformOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);