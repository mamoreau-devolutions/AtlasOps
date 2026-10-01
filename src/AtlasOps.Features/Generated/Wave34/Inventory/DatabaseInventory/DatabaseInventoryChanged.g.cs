namespace AtlasOps.Features.Inventory.DatabaseInventory;

public sealed record DatabaseInventoryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);