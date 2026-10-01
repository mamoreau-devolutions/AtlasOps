namespace AtlasOps.Features.Storage.StorageLifecycleRecovery;

public sealed record StorageLifecycleRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);