namespace AtlasOps.Features.Platform.FeatureDiscovery;

public sealed record UpdateFeatureDiscoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);