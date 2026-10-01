namespace AtlasOps.Features.Compute.ComputeLifecycleProvisioning;

public sealed record ComputeLifecycleProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);