namespace AtlasOps.Features.Network.NetworkAddressRecovery;

public sealed record UpdateNetworkAddressRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);