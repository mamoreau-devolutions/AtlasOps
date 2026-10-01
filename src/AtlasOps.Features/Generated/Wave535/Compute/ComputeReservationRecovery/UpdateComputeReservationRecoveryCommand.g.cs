namespace AtlasOps.Features.Compute.ComputeReservationRecovery;

public sealed record UpdateComputeReservationRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);