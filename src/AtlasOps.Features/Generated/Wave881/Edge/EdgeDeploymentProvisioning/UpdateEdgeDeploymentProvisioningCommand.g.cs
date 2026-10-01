namespace AtlasOps.Features.Edge.EdgeDeploymentProvisioning;

public sealed record UpdateEdgeDeploymentProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);