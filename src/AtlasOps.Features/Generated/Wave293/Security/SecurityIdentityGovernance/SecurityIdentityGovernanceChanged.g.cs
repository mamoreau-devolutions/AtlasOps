namespace AtlasOps.Features.Security.SecurityIdentityGovernance;

public sealed record SecurityIdentityGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);