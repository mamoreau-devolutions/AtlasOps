namespace AtlasOps.Features.Security.SecurityKeyOptimization;

public sealed record UpdateSecurityKeyOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);