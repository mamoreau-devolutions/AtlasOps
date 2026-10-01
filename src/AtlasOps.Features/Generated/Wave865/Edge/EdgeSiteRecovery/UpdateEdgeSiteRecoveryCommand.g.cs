namespace AtlasOps.Features.Edge.EdgeSiteRecovery;

public sealed record UpdateEdgeSiteRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);