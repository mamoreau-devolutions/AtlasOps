namespace AtlasOps.Features.Compute.ComputeReservationOptimization;

public sealed record ComputeReservationOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);