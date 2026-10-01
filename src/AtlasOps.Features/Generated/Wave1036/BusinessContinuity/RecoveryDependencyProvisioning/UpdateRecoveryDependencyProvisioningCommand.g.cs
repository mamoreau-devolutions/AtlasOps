namespace AtlasOps.Features.BusinessContinuity.RecoveryDependencyProvisioning;

public sealed record UpdateRecoveryDependencyProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);