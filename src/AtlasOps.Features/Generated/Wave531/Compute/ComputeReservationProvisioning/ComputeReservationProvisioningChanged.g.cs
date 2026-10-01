namespace AtlasOps.Features.Compute.ComputeReservationProvisioning;

public sealed record ComputeReservationProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);