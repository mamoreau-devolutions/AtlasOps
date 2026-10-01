namespace AtlasOps.Features.Inventory.CertificateInventory;

public sealed record UpdateCertificateInventoryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);