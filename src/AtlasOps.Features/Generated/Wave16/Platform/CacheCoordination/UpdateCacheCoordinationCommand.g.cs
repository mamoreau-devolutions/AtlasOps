namespace AtlasOps.Features.Platform.CacheCoordination;

public sealed record UpdateCacheCoordinationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);