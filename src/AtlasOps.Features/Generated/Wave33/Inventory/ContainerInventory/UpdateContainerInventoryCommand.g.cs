namespace AtlasOps.Features.Inventory.ContainerInventory;

public sealed record UpdateContainerInventoryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);