namespace AtlasOps.Features.Edge.EdgeNetworkProvisioning;

public sealed record UpdateEdgeNetworkProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);