namespace AtlasOps.Features.BusinessContinuity.RecoveryRunbookGovernance;

public sealed record RecoveryRunbookGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);