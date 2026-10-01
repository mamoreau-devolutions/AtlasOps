namespace AtlasOps.Features.Security.SecurityFindingOptimization;

public sealed record SecurityFindingOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);