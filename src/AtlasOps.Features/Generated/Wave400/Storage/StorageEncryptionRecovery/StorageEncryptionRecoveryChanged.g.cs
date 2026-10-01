namespace AtlasOps.Features.Storage.StorageEncryptionRecovery;

public sealed record StorageEncryptionRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);