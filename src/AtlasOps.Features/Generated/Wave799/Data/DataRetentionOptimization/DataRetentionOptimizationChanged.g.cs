namespace AtlasOps.Features.Data.DataRetentionOptimization;

public sealed record DataRetentionOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);