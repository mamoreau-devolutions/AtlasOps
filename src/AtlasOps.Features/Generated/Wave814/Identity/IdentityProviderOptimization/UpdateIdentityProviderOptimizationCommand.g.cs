namespace AtlasOps.Features.Identity.IdentityProviderOptimization;

public sealed record UpdateIdentityProviderOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);