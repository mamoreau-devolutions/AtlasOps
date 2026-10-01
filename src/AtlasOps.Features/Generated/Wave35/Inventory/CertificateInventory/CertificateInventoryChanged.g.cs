namespace AtlasOps.Features.Inventory.CertificateInventory;

public sealed record CertificateInventoryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);