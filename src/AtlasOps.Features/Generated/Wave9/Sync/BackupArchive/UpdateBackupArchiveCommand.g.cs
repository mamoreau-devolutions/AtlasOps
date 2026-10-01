namespace AtlasOps.Features.Sync.BackupArchive;

public sealed record UpdateBackupArchiveCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);