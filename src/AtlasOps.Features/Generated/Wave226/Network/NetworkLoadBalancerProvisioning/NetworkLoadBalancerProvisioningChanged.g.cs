namespace AtlasOps.Features.Network.NetworkLoadBalancerProvisioning;

public sealed record NetworkLoadBalancerProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);