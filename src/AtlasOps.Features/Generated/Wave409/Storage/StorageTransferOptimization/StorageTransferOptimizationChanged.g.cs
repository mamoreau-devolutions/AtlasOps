namespace AtlasOps.Features.Storage.StorageTransferOptimization;

public sealed record StorageTransferOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);