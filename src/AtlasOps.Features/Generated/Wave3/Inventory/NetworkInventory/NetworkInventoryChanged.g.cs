namespace AtlasOps.Features.Inventory.NetworkInventory;

public sealed record NetworkInventoryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);