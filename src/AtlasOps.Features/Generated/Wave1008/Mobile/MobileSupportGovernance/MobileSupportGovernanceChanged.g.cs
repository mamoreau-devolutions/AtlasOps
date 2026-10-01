namespace AtlasOps.Features.Mobile.MobileSupportGovernance;

public sealed record MobileSupportGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);