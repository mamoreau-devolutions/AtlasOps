namespace AtlasOps.Features.Platform.FeatureToggle;

public sealed record FeatureToggleChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);