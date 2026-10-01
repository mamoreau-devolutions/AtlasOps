namespace AtlasOps.Features.Editor.SnippetCatalog;

public sealed record UpdateSnippetCatalogCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);