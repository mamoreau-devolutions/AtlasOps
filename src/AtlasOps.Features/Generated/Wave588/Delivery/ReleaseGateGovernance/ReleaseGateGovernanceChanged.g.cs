namespace AtlasOps.Features.Delivery.ReleaseGateGovernance;

public sealed record ReleaseGateGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);