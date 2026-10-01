namespace AtlasOps.Features.Inventory.CostObservation;

public sealed record UpdateCostObservationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);