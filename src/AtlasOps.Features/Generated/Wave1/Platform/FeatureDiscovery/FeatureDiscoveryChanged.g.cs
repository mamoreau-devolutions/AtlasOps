namespace AtlasOps.Features.Platform.FeatureDiscovery;

public sealed record FeatureDiscoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);