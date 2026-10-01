namespace AtlasOps.Features.Platform.FeatureToggle;

public sealed record UpdateFeatureToggleCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);