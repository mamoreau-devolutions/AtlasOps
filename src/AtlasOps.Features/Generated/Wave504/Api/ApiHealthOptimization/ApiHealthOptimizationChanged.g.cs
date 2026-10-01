namespace AtlasOps.Features.Api.ApiHealthOptimization;

public sealed record ApiHealthOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);