namespace AtlasOps.Features.Inventory.ServiceInventory;

public sealed record ServiceInventoryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);