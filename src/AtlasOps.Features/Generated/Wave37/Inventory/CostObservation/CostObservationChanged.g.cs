namespace AtlasOps.Features.Inventory.CostObservation;

public sealed record CostObservationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);