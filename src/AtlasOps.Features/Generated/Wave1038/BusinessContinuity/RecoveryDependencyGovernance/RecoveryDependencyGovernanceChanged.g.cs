namespace AtlasOps.Features.BusinessContinuity.RecoveryDependencyGovernance;

public sealed record RecoveryDependencyGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);