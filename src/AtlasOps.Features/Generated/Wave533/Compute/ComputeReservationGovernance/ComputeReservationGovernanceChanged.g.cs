namespace AtlasOps.Features.Compute.ComputeReservationGovernance;

public sealed record ComputeReservationGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);