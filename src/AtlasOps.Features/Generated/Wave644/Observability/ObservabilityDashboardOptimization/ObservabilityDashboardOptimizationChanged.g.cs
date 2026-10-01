namespace AtlasOps.Features.Observability.ObservabilityDashboardOptimization;

public sealed record ObservabilityDashboardOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);