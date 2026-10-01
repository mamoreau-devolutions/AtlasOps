namespace AtlasOps.Features.Sync.BackupArchive;

public sealed record BackupArchiveChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);