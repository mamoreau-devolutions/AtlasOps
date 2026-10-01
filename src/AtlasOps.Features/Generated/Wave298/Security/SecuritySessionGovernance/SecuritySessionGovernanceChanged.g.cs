namespace AtlasOps.Features.Security.SecuritySessionGovernance;

public sealed record SecuritySessionGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);