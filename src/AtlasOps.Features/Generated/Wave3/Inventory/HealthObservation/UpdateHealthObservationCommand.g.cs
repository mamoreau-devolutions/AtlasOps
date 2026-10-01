namespace AtlasOps.Features.Inventory.HealthObservation;

public sealed record UpdateHealthObservationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);