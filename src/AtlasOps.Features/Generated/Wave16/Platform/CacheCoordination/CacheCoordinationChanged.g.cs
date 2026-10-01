namespace AtlasOps.Features.Platform.CacheCoordination;

public sealed record CacheCoordinationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);