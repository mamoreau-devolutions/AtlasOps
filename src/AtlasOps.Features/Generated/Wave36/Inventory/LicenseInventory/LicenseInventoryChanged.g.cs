namespace AtlasOps.Features.Inventory.LicenseInventory;

public sealed record LicenseInventoryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);