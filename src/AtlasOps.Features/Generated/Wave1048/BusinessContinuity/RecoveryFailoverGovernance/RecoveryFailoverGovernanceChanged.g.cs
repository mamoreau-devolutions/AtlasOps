namespace AtlasOps.Features.BusinessContinuity.RecoveryFailoverGovernance;

public sealed record RecoveryFailoverGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);