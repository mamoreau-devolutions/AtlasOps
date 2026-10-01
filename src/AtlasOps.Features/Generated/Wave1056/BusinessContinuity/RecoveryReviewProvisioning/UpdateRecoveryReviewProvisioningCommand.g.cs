namespace AtlasOps.Features.BusinessContinuity.RecoveryReviewProvisioning;

public sealed record UpdateRecoveryReviewProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);