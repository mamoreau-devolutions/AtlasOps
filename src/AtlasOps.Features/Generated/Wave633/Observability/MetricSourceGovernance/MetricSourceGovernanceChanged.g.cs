namespace AtlasOps.Features.Observability.MetricSourceGovernance;

public sealed record MetricSourceGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);