namespace AtlasOps.Features.BusinessContinuity.RecoveryEvidenceOptimization;

public sealed record UpdateRecoveryEvidenceOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);