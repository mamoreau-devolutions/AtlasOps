namespace AtlasOps.Features.Sync.DataArchive;

public sealed record DataArchiveChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);