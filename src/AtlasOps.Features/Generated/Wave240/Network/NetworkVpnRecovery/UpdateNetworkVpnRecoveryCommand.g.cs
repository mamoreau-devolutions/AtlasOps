namespace AtlasOps.Features.Network.NetworkVpnRecovery;

public sealed record UpdateNetworkVpnRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);