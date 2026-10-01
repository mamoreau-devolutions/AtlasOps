namespace AtlasOps.Features.Delivery.ReleaseEnvironmentProvisioning;

public sealed record ReleaseEnvironmentProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);