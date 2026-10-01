namespace AtlasOps.Features.Sync.SyncBatch;

public sealed record SyncBatchChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);