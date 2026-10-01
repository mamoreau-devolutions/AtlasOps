namespace AtlasOps.Features.Inventory.DiscoveryScan;

public sealed record DiscoveryScanChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);