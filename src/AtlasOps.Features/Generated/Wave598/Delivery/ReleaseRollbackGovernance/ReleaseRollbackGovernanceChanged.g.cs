namespace AtlasOps.Features.Delivery.ReleaseRollbackGovernance;

public sealed record ReleaseRollbackGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);