namespace AtlasOps.Features.ServiceManagement.ServiceScorecardGovernance;

public sealed record ServiceScorecardGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);