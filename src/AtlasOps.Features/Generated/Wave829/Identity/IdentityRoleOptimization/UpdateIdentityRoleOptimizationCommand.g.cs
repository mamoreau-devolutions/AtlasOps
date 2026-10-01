namespace AtlasOps.Features.Identity.IdentityRoleOptimization;

public sealed record UpdateIdentityRoleOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);