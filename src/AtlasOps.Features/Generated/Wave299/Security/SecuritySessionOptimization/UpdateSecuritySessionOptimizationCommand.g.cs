namespace AtlasOps.Features.Security.SecuritySessionOptimization;

public sealed record UpdateSecuritySessionOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);