namespace AtlasOps.Features.Mobile.MobileApplicationGovernance;

public sealed record MobileApplicationGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);