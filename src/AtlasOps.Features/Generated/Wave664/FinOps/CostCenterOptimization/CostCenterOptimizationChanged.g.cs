namespace AtlasOps.Features.FinOps.CostCenterOptimization;

public sealed record CostCenterOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);