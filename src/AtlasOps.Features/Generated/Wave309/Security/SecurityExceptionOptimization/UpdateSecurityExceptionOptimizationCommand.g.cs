namespace AtlasOps.Features.Security.SecurityExceptionOptimization;

public sealed record UpdateSecurityExceptionOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);