namespace AtlasOps.Features.Cloud.CloudStorageOptimization;

public sealed record CloudStorageOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);