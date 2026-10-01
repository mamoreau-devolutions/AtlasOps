namespace AtlasOps.Features.Delivery.ReleaseMetricProvisioning;

public sealed record ReleaseMetricProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);