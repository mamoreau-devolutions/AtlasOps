namespace AtlasOps.Features.BusinessContinuity.RecoveryFailoverOptimization;

public sealed record UpdateRecoveryFailoverOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);