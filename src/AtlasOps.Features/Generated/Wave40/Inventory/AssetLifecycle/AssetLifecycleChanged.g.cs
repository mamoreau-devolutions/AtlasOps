namespace AtlasOps.Features.Inventory.AssetLifecycle;

public sealed record AssetLifecycleChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);