namespace AtlasOps.Features.BusinessContinuity.RecoveryRunbookOptimization;

public sealed record UpdateRecoveryRunbookOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);