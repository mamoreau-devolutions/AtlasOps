namespace AtlasOps.Features.Security.SecurityIdentityOptimization;

public sealed record UpdateSecurityIdentityOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);