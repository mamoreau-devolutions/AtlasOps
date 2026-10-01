namespace AtlasOps.Features.Editor.SchemaBrowser;

public sealed record SchemaBrowserChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);