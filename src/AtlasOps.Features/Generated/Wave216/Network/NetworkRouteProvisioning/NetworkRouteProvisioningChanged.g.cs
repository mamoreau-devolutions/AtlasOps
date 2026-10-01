namespace AtlasOps.Features.Network.NetworkRouteProvisioning;

public sealed record NetworkRouteProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);