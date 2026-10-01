namespace AtlasOps.Features.BusinessContinuity.RecoveryBackupOptimization;

public sealed record RecoveryBackupOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);