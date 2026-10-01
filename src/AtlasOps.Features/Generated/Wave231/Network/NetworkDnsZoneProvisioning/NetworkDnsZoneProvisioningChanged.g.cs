namespace AtlasOps.Features.Network.NetworkDnsZoneProvisioning;

public sealed record NetworkDnsZoneProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);