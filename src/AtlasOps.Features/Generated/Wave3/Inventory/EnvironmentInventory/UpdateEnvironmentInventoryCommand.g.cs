namespace AtlasOps.Features.Inventory.EnvironmentInventory;

public sealed record UpdateEnvironmentInventoryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);