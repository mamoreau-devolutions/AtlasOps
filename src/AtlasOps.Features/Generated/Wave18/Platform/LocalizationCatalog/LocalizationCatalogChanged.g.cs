namespace AtlasOps.Features.Platform.LocalizationCatalog;

public sealed record LocalizationCatalogChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);