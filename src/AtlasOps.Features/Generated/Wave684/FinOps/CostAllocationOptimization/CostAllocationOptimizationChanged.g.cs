namespace AtlasOps.Features.FinOps.CostAllocationOptimization;

public sealed record CostAllocationOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);