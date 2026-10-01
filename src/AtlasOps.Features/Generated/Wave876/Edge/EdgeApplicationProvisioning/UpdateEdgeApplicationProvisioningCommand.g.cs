namespace AtlasOps.Features.Edge.EdgeApplicationProvisioning;

public sealed record UpdateEdgeApplicationProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);