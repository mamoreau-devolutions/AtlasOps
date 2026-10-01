namespace AtlasOps.Features.Observability.MetricSourceOptimization;

public sealed record MetricSourceOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);