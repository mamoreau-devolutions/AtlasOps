namespace AtlasOps.Features.Data.DataPipelineOptimization;

public sealed record DataPipelineOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);