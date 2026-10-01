namespace AtlasOps.Features.Observability.LogSourceOptimization;

public sealed record LogSourceOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);