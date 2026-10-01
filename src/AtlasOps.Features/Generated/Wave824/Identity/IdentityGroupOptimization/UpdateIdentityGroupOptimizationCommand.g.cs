namespace AtlasOps.Features.Identity.IdentityGroupOptimization;

public sealed record UpdateIdentityGroupOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);