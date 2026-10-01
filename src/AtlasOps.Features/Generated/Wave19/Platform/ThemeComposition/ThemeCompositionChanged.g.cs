namespace AtlasOps.Features.Platform.ThemeComposition;

public sealed record ThemeCompositionChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);