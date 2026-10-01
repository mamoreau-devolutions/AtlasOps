namespace AtlasOps.Features.Security.SecurityBaselineOptimization;

public sealed record UpdateSecurityBaselineOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);