namespace AtlasOps.Features.Storage.StorageEncryptionOptimization;

public sealed record StorageEncryptionOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);