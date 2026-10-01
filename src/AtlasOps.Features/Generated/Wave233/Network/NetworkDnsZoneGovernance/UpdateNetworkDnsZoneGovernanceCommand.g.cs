namespace AtlasOps.Features.Network.NetworkDnsZoneGovernance;

public sealed record UpdateNetworkDnsZoneGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);