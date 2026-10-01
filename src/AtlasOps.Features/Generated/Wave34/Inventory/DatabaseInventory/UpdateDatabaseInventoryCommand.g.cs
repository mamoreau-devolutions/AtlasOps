namespace AtlasOps.Features.Inventory.DatabaseInventory;

public sealed record UpdateDatabaseInventoryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);