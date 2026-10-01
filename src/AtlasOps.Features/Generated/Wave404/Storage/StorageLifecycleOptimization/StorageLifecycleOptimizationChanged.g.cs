namespace AtlasOps.Features.Storage.StorageLifecycleOptimization;

public sealed record StorageLifecycleOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);