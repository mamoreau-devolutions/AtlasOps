namespace AtlasOps.Features.Sync.SyncSchedule;

public sealed record UpdateSyncScheduleCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);