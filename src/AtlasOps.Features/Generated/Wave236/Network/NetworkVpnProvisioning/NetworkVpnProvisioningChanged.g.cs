namespace AtlasOps.Features.Network.NetworkVpnProvisioning;

public sealed record NetworkVpnProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);