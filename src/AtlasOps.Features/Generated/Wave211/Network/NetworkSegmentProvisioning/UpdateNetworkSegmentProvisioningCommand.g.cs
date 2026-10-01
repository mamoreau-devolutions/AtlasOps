namespace AtlasOps.Features.Network.NetworkSegmentProvisioning;

public sealed record UpdateNetworkSegmentProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);