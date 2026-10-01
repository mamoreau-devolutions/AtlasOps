namespace AtlasOps.Features.Inventory.ClusterInventory;

public sealed record ClusterInventoryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);