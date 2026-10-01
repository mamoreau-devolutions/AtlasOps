namespace AtlasOps.Features.Mobile.MobilePolicyGovernance;

public sealed record MobilePolicyGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);