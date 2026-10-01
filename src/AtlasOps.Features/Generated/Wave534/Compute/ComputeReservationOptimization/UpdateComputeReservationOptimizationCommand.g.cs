namespace AtlasOps.Features.Compute.ComputeReservationOptimization;

public sealed record UpdateComputeReservationOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);