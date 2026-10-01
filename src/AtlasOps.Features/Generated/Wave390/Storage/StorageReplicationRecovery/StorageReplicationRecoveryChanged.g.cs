namespace AtlasOps.Features.Storage.StorageReplicationRecovery;

public sealed record StorageReplicationRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);