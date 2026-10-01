namespace AtlasOps.Features.Edge.EdgeIncidentProvisioning;

public sealed record UpdateEdgeIncidentProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);