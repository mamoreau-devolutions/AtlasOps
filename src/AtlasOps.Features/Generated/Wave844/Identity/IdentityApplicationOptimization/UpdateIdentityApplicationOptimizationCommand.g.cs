namespace AtlasOps.Features.Identity.IdentityApplicationOptimization;

public sealed record UpdateIdentityApplicationOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);