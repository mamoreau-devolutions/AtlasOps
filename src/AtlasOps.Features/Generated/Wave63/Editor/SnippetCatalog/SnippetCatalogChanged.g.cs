namespace AtlasOps.Features.Editor.SnippetCatalog;

public sealed record SnippetCatalogChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);