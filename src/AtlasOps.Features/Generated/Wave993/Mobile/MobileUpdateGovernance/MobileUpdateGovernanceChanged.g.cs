namespace AtlasOps.Features.Mobile.MobileUpdateGovernance;

public sealed record MobileUpdateGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);