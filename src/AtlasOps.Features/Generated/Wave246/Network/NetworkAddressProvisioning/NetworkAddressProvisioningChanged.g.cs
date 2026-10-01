namespace AtlasOps.Features.Network.NetworkAddressProvisioning;

public sealed record NetworkAddressProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);