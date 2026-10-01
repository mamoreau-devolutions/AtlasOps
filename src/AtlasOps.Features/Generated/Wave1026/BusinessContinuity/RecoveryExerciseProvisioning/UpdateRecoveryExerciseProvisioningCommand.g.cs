namespace AtlasOps.Features.BusinessContinuity.RecoveryExerciseProvisioning;

public sealed record UpdateRecoveryExerciseProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);