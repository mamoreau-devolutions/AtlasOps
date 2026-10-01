namespace AtlasOps.Features.Sync.SyncBatch;

public sealed record UpdateSyncBatchCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);