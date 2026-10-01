namespace AtlasOps.Features.Observability.LogQueryGovernance;

public sealed record LogQueryGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);