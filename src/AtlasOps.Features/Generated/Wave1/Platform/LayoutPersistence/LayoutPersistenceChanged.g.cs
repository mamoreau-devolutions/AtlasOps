namespace AtlasOps.Features.Platform.LayoutPersistence;

public sealed record LayoutPersistenceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);