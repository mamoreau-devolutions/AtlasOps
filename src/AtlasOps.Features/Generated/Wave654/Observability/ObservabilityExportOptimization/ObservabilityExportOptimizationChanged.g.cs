namespace AtlasOps.Features.Observability.ObservabilityExportOptimization;

public sealed record ObservabilityExportOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);