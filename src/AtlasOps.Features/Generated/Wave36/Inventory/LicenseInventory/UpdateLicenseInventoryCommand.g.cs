namespace AtlasOps.Features.Inventory.LicenseInventory;

public sealed record UpdateLicenseInventoryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);