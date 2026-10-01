namespace AtlasOps.Features.Inventory.ServiceInventory;

public sealed record UpdateServiceInventoryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);