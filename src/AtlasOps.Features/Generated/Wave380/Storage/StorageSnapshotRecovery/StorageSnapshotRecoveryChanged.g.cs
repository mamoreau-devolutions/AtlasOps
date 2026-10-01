namespace AtlasOps.Features.Storage.StorageSnapshotRecovery;

public sealed record StorageSnapshotRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);