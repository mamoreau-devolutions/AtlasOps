namespace AtlasOps.Features.Storage.ObjectBucketOptimization;

public sealed record ObjectBucketOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);