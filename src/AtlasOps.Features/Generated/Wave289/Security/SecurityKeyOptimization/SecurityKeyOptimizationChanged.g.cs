namespace AtlasOps.Features.Security.SecurityKeyOptimization;

public sealed record SecurityKeyOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);