namespace AtlasOps.Features.Sync.ImportExport;

public sealed record ImportExportChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);