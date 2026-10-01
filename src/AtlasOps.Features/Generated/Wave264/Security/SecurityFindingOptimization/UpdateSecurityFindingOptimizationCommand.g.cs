namespace AtlasOps.Features.Security.SecurityFindingOptimization;

public sealed record UpdateSecurityFindingOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);