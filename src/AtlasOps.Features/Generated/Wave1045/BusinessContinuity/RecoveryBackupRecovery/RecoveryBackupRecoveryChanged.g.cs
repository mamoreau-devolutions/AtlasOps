namespace AtlasOps.Features.BusinessContinuity.RecoveryBackupRecovery;

public sealed record RecoveryBackupRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);