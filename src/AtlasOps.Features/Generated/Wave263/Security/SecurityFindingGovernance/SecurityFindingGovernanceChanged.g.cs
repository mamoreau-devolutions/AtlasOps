namespace AtlasOps.Features.Security.SecurityFindingGovernance;

public sealed record SecurityFindingGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);