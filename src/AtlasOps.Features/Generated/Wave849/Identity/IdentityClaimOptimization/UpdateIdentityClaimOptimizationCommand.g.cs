namespace AtlasOps.Features.Identity.IdentityClaimOptimization;

public sealed record UpdateIdentityClaimOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);