namespace AtlasOps.Features.Delivery.ReleaseGateProvisioning;

public sealed record ReleaseGateProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);