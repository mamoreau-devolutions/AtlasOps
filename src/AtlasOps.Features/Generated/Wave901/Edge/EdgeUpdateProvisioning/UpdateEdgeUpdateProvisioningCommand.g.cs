namespace AtlasOps.Features.Edge.EdgeUpdateProvisioning;

public sealed record UpdateEdgeUpdateProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);