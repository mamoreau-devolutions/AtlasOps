namespace AtlasOps.Features.BusinessContinuity.RecoveryBackupProvisioning;

public sealed record RecoveryBackupProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);