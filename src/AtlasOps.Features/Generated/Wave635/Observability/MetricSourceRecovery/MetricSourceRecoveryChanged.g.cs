namespace AtlasOps.Features.Observability.MetricSourceRecovery;

public sealed record MetricSourceRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);