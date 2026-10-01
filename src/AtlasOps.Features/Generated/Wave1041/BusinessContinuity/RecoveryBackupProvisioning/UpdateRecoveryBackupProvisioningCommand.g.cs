namespace AtlasOps.Features.BusinessContinuity.RecoveryBackupProvisioning;

public sealed record UpdateRecoveryBackupProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);