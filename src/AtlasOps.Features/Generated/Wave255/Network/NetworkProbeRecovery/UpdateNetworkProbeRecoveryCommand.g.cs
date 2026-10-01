namespace AtlasOps.Features.Network.NetworkProbeRecovery;

public sealed record UpdateNetworkProbeRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);