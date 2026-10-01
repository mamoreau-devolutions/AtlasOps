namespace AtlasOps.Features.Observability.MetricAlertGovernance;

public sealed record MetricAlertGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);