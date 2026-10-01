namespace AtlasOps.Features.Observability.TraceSourceOptimization;

public sealed record TraceSourceOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);