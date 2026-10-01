namespace AtlasOps.Features.Observability.LogSourceGovernance;

public sealed record LogSourceGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);