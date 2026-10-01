namespace AtlasOps.Features.BusinessContinuity.RecoveryFailoverProvisioning;

public sealed record UpdateRecoveryFailoverProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);