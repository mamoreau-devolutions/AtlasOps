namespace AtlasOps.Features.BusinessContinuity.RecoveryReviewGovernance;

public sealed record UpdateRecoveryReviewGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);