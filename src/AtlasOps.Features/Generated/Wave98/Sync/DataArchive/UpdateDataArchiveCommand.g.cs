namespace AtlasOps.Features.Sync.DataArchive;

public sealed record UpdateDataArchiveCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);