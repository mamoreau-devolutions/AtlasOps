namespace AtlasOps.Features.BusinessContinuity.RecoveryEvidenceRecovery;

public sealed record RecoveryEvidenceRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);