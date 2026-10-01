namespace AtlasOps.Features.Network.NetworkDnsZoneRecovery;

public sealed record UpdateNetworkDnsZoneRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);