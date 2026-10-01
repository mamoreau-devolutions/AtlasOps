namespace AtlasOps.Features.Network.NetworkPolicyProvisioning;

public sealed record NetworkPolicyProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);