namespace AtlasOps.Features.Connections.ConnectionImport;

public sealed record ConnectionImportChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);