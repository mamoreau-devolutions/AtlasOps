namespace AtlasOps.Features.Edge.EdgeDeviceProvisioning;

public sealed record UpdateEdgeDeviceProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);