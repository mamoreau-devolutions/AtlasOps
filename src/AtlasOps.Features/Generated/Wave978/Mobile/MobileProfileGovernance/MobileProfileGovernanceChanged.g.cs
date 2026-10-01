namespace AtlasOps.Features.Mobile.MobileProfileGovernance;

public sealed record MobileProfileGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);