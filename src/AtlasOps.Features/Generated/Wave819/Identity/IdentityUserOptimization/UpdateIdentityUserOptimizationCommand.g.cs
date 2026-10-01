namespace AtlasOps.Features.Identity.IdentityUserOptimization;

public sealed record UpdateIdentityUserOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);