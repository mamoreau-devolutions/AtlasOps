namespace AtlasOps.Features.Compute.ComputeMetricProvisioning;

public sealed record ComputeMetricProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);