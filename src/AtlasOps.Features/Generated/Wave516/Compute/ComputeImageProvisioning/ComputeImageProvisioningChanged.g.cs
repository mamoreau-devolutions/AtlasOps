namespace AtlasOps.Features.Compute.ComputeImageProvisioning;

public sealed record ComputeImageProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);