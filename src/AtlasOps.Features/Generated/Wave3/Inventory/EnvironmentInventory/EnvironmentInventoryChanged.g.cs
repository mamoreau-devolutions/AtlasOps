namespace AtlasOps.Features.Inventory.EnvironmentInventory;

public sealed record EnvironmentInventoryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);