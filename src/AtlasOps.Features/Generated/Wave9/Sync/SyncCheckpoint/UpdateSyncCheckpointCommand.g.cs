namespace AtlasOps.Features.Sync.SyncCheckpoint;

public sealed record UpdateSyncCheckpointCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);