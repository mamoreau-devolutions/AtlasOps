namespace AtlasOps.Features.Sync.TombstoneRecord;

public sealed record UpdateTombstoneRecordCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);