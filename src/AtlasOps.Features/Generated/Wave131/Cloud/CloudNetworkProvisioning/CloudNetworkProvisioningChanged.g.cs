namespace AtlasOps.Features.Cloud.CloudNetworkProvisioning;

public sealed record CloudNetworkProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);