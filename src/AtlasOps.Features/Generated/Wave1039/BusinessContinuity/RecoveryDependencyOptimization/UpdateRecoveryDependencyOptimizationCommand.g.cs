namespace AtlasOps.Features.BusinessContinuity.RecoveryDependencyOptimization;

public sealed record UpdateRecoveryDependencyOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);