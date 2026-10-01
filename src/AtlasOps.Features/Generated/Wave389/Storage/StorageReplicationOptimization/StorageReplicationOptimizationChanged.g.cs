namespace AtlasOps.Features.Storage.StorageReplicationOptimization;

public sealed record StorageReplicationOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);