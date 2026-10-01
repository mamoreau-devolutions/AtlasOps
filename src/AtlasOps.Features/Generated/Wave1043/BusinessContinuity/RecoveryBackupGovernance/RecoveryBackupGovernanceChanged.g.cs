namespace AtlasOps.Features.BusinessContinuity.RecoveryBackupGovernance;

public sealed record RecoveryBackupGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);