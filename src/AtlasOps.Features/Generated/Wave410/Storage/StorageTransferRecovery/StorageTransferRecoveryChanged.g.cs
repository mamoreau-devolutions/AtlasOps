namespace AtlasOps.Features.Storage.StorageTransferRecovery;

public sealed record StorageTransferRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);