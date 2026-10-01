namespace AtlasOps.Features.Inventory.NetworkInventory;

public sealed record UpdateNetworkInventoryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);