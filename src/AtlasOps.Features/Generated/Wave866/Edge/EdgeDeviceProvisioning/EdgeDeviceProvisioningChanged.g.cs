namespace AtlasOps.Features.Edge.EdgeDeviceProvisioning;

public sealed record EdgeDeviceProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);