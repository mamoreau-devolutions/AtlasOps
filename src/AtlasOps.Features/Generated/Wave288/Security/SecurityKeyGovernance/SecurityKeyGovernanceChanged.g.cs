namespace AtlasOps.Features.Security.SecurityKeyGovernance;

public sealed record SecurityKeyGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);