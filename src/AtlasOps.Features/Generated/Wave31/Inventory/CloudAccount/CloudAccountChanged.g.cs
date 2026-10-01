namespace AtlasOps.Features.Inventory.CloudAccount;

public sealed record CloudAccountChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);