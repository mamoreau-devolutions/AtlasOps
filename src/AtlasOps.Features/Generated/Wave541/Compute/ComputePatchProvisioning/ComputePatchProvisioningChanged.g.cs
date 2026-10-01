namespace AtlasOps.Features.Compute.ComputePatchProvisioning;

public sealed record ComputePatchProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);