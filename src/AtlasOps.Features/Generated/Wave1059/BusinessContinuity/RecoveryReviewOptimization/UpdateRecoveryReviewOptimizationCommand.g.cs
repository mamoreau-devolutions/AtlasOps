namespace AtlasOps.Features.BusinessContinuity.RecoveryReviewOptimization;

public sealed record UpdateRecoveryReviewOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);