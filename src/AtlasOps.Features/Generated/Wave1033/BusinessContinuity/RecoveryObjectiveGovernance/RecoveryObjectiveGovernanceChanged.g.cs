namespace AtlasOps.Features.BusinessContinuity.RecoveryObjectiveGovernance;

public sealed record RecoveryObjectiveGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);