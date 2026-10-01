namespace AtlasOps.Features.Editor.DataExport;

public sealed record DataExportChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);