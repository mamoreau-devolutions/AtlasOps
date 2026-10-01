namespace AtlasOps.Features.Inventory.HealthObservation;

public sealed record HealthObservationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);