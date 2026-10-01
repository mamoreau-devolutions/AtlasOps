namespace AtlasOps.Features.Delivery.ReleaseMetricOptimization;

public sealed record ReleaseMetricOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);