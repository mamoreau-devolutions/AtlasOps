namespace AtlasOps.Features.Compute.ComputeReservationRecovery;

public sealed record ComputeReservationRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);