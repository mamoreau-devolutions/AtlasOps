namespace AtlasOps.Features.BusinessContinuity.RecoveryEvidenceProvisioning;

public sealed record UpdateRecoveryEvidenceProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);