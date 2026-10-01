namespace AtlasOps.Features.Mobile.MobileFleetGovernance;

public sealed record MobileFleetGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);