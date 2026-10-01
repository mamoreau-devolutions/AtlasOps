namespace AtlasOps.Features.Security.SecurityScanOptimization;

public sealed record UpdateSecurityScanOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);