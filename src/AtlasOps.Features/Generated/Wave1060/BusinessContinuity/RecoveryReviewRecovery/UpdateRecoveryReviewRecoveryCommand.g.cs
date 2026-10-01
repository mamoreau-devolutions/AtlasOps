namespace AtlasOps.Features.BusinessContinuity.RecoveryReviewRecovery;

public sealed record UpdateRecoveryReviewRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);