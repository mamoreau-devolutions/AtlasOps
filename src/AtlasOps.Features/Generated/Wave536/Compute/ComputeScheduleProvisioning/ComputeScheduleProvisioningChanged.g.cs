namespace AtlasOps.Features.Compute.ComputeScheduleProvisioning;

public sealed record ComputeScheduleProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);