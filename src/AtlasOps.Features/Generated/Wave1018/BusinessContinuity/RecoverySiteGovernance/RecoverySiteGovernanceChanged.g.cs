namespace AtlasOps.Features.BusinessContinuity.RecoverySiteGovernance;

public sealed record RecoverySiteGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);