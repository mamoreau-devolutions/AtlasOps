namespace AtlasOps.Features.Data.DataProductOptimization;

public sealed record DataProductOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);