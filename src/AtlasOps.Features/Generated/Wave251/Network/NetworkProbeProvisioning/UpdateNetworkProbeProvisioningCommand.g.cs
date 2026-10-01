namespace AtlasOps.Features.Network.NetworkProbeProvisioning;

public sealed record UpdateNetworkProbeProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);