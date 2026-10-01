namespace AtlasOps.Features.Sync.SyncCheckpoint;

public sealed record SyncCheckpointChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);