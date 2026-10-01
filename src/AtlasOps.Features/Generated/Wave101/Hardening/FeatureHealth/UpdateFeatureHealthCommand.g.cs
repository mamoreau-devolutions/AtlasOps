namespace AtlasOps.Features.Hardening.FeatureHealth;

public sealed record UpdateFeatureHealthCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);