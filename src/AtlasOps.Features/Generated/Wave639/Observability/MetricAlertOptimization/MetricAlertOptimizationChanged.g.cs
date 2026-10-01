namespace AtlasOps.Features.Observability.MetricAlertOptimization;

public sealed record MetricAlertOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);