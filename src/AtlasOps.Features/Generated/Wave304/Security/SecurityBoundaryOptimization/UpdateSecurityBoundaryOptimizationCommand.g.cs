namespace AtlasOps.Features.Security.SecurityBoundaryOptimization;

public sealed record UpdateSecurityBoundaryOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);