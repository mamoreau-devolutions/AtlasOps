namespace AtlasOps.Features.Storage.StorageArchiveRecovery;

public sealed record StorageArchiveRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);