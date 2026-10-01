namespace AtlasOps.Features.Network.NetworkDnsZoneProvisioning;

public sealed record UpdateNetworkDnsZoneProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);