namespace AtlasOps.Features.Identity.IdentitySessionOptimization;

public sealed record UpdateIdentitySessionOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);