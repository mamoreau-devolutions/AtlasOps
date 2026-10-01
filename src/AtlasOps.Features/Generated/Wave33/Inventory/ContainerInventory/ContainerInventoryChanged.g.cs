namespace AtlasOps.Features.Inventory.ContainerInventory;

public sealed record ContainerInventoryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);