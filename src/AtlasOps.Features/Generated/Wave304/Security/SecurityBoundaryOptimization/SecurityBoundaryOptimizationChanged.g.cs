namespace AtlasOps.Features.Security.SecurityBoundaryOptimization;

public sealed record SecurityBoundaryOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);