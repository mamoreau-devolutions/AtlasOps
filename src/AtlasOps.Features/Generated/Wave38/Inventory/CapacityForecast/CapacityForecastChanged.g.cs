namespace AtlasOps.Features.Inventory.CapacityForecast;

public sealed record CapacityForecastChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);