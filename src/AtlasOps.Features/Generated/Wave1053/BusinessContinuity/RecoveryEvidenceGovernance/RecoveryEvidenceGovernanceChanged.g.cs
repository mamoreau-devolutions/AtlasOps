namespace AtlasOps.Features.BusinessContinuity.RecoveryEvidenceGovernance;

public sealed record RecoveryEvidenceGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);