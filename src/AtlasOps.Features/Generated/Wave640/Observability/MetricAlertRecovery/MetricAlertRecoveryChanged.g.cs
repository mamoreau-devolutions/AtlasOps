namespace AtlasOps.Features.Observability.MetricAlertRecovery;

public sealed record MetricAlertRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);