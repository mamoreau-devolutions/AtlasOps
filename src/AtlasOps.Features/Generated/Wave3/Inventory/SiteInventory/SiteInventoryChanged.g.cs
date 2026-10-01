namespace AtlasOps.Features.Inventory.SiteInventory;

public sealed record SiteInventoryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);