namespace AtlasOps.Features.Security.SecurityPatchOptimization;

public sealed record UpdateSecurityPatchOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);