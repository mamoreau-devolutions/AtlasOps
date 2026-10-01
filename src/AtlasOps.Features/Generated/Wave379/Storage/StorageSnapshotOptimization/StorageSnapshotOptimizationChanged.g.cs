namespace AtlasOps.Features.Storage.StorageSnapshotOptimization;

public sealed record StorageSnapshotOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);