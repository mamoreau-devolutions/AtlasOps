namespace AtlasOps.Features.Platform.LocalizationCatalog;

public sealed record UpdateLocalizationCatalogCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);