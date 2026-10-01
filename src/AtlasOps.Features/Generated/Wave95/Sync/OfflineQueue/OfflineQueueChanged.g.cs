namespace AtlasOps.Features.Sync.OfflineQueue;

public sealed record OfflineQueueChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);