namespace AtlasOps.Features.Identity.IdentityLifecycleOptimization;

public sealed record UpdateIdentityLifecycleOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);