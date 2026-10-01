namespace AtlasOps.Features.Compute.ComputeReservationMonitoring;

public sealed record UpdateComputeReservationMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);