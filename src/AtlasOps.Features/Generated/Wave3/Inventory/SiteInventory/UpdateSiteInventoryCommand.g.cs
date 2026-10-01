namespace AtlasOps.Features.Inventory.SiteInventory;

public sealed record UpdateSiteInventoryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);