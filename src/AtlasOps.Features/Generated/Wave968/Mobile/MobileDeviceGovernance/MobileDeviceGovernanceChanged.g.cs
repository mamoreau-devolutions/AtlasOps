namespace AtlasOps.Features.Mobile.MobileDeviceGovernance;

public sealed record MobileDeviceGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);