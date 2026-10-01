namespace AtlasOps.Features.Security.SecurityBaselineGovernance;

public sealed record SecurityBaselineGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);