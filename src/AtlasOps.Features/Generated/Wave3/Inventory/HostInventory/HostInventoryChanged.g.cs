namespace AtlasOps.Features.Inventory.HostInventory;

public sealed record HostInventoryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);