namespace AtlasOps.Features.Network.NetworkPeerProvisioning;

public sealed record NetworkPeerProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);