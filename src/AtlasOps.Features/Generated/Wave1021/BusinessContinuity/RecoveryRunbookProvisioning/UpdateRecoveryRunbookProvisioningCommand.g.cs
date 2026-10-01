namespace AtlasOps.Features.BusinessContinuity.RecoveryRunbookProvisioning;

public sealed record UpdateRecoveryRunbookProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);