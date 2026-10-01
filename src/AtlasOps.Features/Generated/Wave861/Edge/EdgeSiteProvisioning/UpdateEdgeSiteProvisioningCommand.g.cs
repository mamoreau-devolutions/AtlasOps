namespace AtlasOps.Features.Edge.EdgeSiteProvisioning;

public sealed record UpdateEdgeSiteProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);