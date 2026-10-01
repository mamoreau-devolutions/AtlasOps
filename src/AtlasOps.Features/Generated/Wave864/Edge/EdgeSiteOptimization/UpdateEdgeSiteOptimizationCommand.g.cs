namespace AtlasOps.Features.Edge.EdgeSiteOptimization;

public sealed record UpdateEdgeSiteOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);