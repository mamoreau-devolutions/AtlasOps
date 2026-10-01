namespace AtlasOps.Features.FinOps.CostAnomalyOptimization;

public sealed record CostAnomalyOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);