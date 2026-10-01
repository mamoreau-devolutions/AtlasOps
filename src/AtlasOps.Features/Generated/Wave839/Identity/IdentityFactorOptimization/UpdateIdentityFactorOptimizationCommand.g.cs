namespace AtlasOps.Features.Identity.IdentityFactorOptimization;

public sealed record UpdateIdentityFactorOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);