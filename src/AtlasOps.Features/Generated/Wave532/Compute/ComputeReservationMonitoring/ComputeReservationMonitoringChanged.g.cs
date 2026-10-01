namespace AtlasOps.Features.Compute.ComputeReservationMonitoring;

public sealed record ComputeReservationMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);