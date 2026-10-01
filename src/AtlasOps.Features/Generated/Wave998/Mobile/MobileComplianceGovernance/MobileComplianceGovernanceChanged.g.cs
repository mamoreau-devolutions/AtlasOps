namespace AtlasOps.Features.Mobile.MobileComplianceGovernance;

public sealed record MobileComplianceGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);