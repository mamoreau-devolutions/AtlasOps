namespace AtlasOps.Features.Observability.TraceSpanOptimization;

public sealed record TraceSpanOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);