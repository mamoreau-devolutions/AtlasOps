namespace AtlasOps.Features.BusinessContinuity.RecoveryObjectiveOptimization;

public sealed record UpdateRecoveryObjectiveOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);