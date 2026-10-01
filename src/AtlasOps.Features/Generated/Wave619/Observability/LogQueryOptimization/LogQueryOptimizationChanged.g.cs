namespace AtlasOps.Features.Observability.LogQueryOptimization;

public sealed record LogQueryOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);