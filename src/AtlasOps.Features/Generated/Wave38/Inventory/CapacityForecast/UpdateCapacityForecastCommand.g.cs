namespace AtlasOps.Features.Inventory.CapacityForecast;

public sealed record UpdateCapacityForecastCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);